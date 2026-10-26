using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Dtos.Extensions;
using SMS.Generic.Enums;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for an invoice detail line item.
    /// </summary>
    public class InvoiceDetailDto : BaseDto
    {
        private static long _jobIdCounter = 0;
        private static int _jobIdInitialized = 0;

        private static string NextJobId()
        {
            if (Interlocked.CompareExchange(ref _jobIdInitialized, 1, 0) == 0)
            {
                var seed = RandomNumberGenerator.GetInt32(1_000_000, 10_000_000); // 7 digits
                Interlocked.Exchange(ref _jobIdCounter, seed - 1L);
            }

            return Interlocked.Increment(ref _jobIdCounter).ToString();
        }

        public InvoiceDetailDto()
        {
        }

        /// <summary>
        /// Creates an invoice detail from a post position and related closed/equipment data.
        /// Performs date filter, hours/vehicle/equipment fee calculation, and officer name lookup.
        /// </summary>
        /// <returns>The created detail, or null if the post position is outside the date range.</returns>
        public static InvoiceDetailDto? FromPostPosition(
            PostPositionDto pp,
            DateOnly startDate,
            DateOnly endDate,
            ClosedPostPositionDto? closed,
            IReadOnlyList<ClosedPostPositionEquipmentDto> closedEquipments,
            IReadOnlyDictionary<Guid?, DepartmentEquipmentDto> deptEquipDict,
            IReadOnlyDictionary<Guid, EmployeeDto> employeeDict,
            RequestDto request,
            PostDto postDto,
            bool enableLiquorLicenseTracking,
            IReadOnlyDictionary<DateOnly, IReadOnlyList<HolidayDayDto>>? holidaysByDate = null,
            EmployeeTypeDto? postEmployeeType = null)
        {
            if (pp.DateOfWork < startDate || pp.DateOfWork > endDate)
                return null;

            var workDate = pp.DateOfWork.Value;
            decimal? hoursWorked = closed != null ? closed.GetBillableHours() : null;
            var billableHoursForUsage = closed?.IsMinimumDue == true ? closed.GetBillableHours() : (decimal?)null;

            decimal vehicleCharge = 0m;
            decimal equipmentCharge = 0m;
            VehicleOwnedByType? vehicleOwnedBy = null;

            if (pp.PostPositionID.HasValue && closed != null)
            {
                foreach (var cpe in closedEquipments.Where(x => x.ClosedPostPositionID == closed.ClosedPostPositionID))
                {
                    if (!deptEquipDict.TryGetValue(cpe.DepartmentEquipmentID, out var equip) || equip == null)
                        continue;
                    var charge = equip.Charge ?? 0m;

                    if (equip.EquipmentType?.IsVehicle == true)
                    {
                        vehicleOwnedBy ??= cpe.VehicleOwnedBy;
                        vehicleCharge += CalculateVehicleEquipmentFee(cpe, equip, postDto.IsEscortActivityType, billableHoursForUsage);
                    }
                    else
                    {
                        if (cpe.EquipmentUsedHours.HasValue)
                        {
                            var hours = billableHoursForUsage ?? (cpe.EquipmentUsedHours.Value / 60m);
                            equipmentCharge += hours * charge;
                        }
                        else if (cpe.UsedEquipmentQuantity.HasValue)
                        {
                            equipmentCharge += cpe.UsedEquipmentQuantity.Value * charge;
                        }
                    }
                }
            }

            string? officerName;
            if (pp.IsPlaceholder)
                officerName = pp.PlaceholderDetail.GetPlaceHolderName();
            else
                officerName = employeeDict.TryGetValue(pp.EmployeeID, out var emp)
                    ? emp.GetFullName()
                    : null;

            decimal? appliedSpecialRate = null;
            var employeeType = postEmployeeType ?? (employeeDict.TryGetValue(pp.EmployeeID, out var assignedEmployee) ? assignedEmployee.EmployeeType : null);
            var basePayRate = pp.PostPositionPayRate;
            var invoiceRate = ResolveInvoicePayRate(basePayRate, workDate, employeeType, holidaysByDate);
            if (enableLiquorLicenseTracking
                && postDto.IsAlcoholServedOrConsumed
                && request.AlcoholPermitRequired
                && (request.SpecialRate ?? 0m) > 0m)
            {
                appliedSpecialRate = request.SpecialRate;
                invoiceRate += appliedSpecialRate ?? 0m;
            }

            var detail = new InvoiceDetailDto(
                jobId: NextJobId(),
                jobName: request.Title,
                jobDate: pp.DateOfWork,
                jobTime: pp.PostStartTime,
                officerName: officerName,
                rate: invoiceRate,
                hours: hoursWorked,
                vehicleFeeAmount: vehicleCharge,
                equipmentFeeAmount: equipmentCharge)
            {
                PostLength = postDto.PostLength,
                ClosePostPositionID = closed?.ClosedPostPositionID,
                WorkEndTime = closed?.WorkEndTime,
                VehicleOwnedBy = vehicleOwnedBy,
                SpecialRate = appliedSpecialRate,
                PostID = postDto.PostID,
                IsMinimumDue = closed?.IsMinimumDue == true
            };

            return detail;
        }

        /// <summary>
        /// Vehicle fee for one closed equipment line: mileage + usage.
        /// Non-escort, employee-owned, and 20 miles or less → 0.
        /// </summary>
        public static decimal CalculateVehicleEquipmentFee(
            ClosedPostPositionEquipmentDto cpe,
            DepartmentEquipmentDto equip,
            bool isEscortActivityType,
            decimal? billableHoursForUsage = null)
        {
            if (equip.EquipmentType?.IsVehicle != true)
                return 0m;

            if (!isEscortActivityType && cpe.VehicleOwnedBy == VehicleOwnedByType.Employee && (cpe.TotalMileage ?? 0m) <= equip.MileageCondition)
                return 0m;

            var charge = equip.Charge ?? 0m;
            decimal vehicleFee = 0m;

            if (equip.ChargeMileage == true)
            {
                var chargeableMileage = cpe.TotalMileage ?? 0m;
                var mileageCharge = chargeableMileage * (equip.MileageCharge ?? 0m);
                vehicleFee += CurrencyRoundHelper.RoundTwo(mileageCharge);
            }

            if (cpe.VehicleUsedHours.HasValue)
            {
                var hours = billableHoursForUsage ?? (cpe.VehicleUsedHours.Value / 60m);
                vehicleFee += hours * charge;
            }
            else if (cpe.VehicleQuantity.HasValue)
                vehicleFee += cpe.VehicleQuantity.Value * charge;

            return CurrencyRoundHelper.RoundTwo(vehicleFee);
        }

        public InvoiceDetailDto(
            string? jobId,
            string? jobName,
            DateOnly? jobDate,
            TimeOnly? jobTime,
            string? officerName,
            decimal? rate,
            decimal? hours,
            decimal? vehicleFeeAmount,
            decimal? equipmentFeeAmount)
        {
            JobId = jobId;
            JobName = jobName;
            JobDate = jobDate;
            JobTime = jobTime;
            OfficerName = officerName;
            Rate = rate;
            Hours = hours;
            VehicleFeeAmount = CurrencyRoundHelper.RoundTwo(vehicleFeeAmount);
            EquipmentFeeAmount = CurrencyRoundHelper.RoundTwo(equipmentFeeAmount);
            TotalAmount = GetTotalAmount();
        }

        /// <summary>
        /// Gets or sets the unique identifier for the invoice detail.
        /// </summary>
        public Guid InvoiceDetailID { get; set; }

        /// <summary>
        /// Gets or sets the parent invoice identifier.
        /// </summary>
        public Guid InvoicesID { get; set; }

        /// <summary>
        /// Gets or sets the closed post position this line item is sourced from, when applicable.
        /// </summary>
        public Guid? ClosePostPositionID { get; set; }

        public TimeOnly? WorkEndTime { get; set; }

        /// <summary>
        /// Gets or sets the financial impact type for this invoice line.
        /// Use in UI: <see cref="FinancialImpactType.None"/> = original; <see cref="FinancialImpactType.Modified"/> = manual edit; <see cref="FinancialImpactType.Credit"/> = re-close line.
        /// </summary>
        public FinancialImpactType FinancialImpactType { get; set; }

        /// <summary>
        /// Populated on view-previous responses only: the invoice detail ID of the current display row this previous version belongs to.
        /// </summary>
        public Guid? CurrentInvoiceDetailId { get; set; }

        /// <summary>
        /// Populated on read from linked <see cref="ClosePostPositionID"/> (not stored on InvoiceDetail).
        /// Contains closed post position details for invoice UI display.
        /// </summary>
        public ClosedPostPositionDto? ClosedPostPosition { get; set; }

        /// <summary>
        /// Gets or sets the job identifier.
        /// </summary>
        [MaxLength(50, ErrorMessage = "JobId cannot exceed 50 characters")]
        public string? JobId { get; set; }

        /// <summary>
        /// Gets or sets the job name.
        /// </summary>
        [MaxLength(500, ErrorMessage = "JobName cannot exceed 500 characters")]
        public string? JobName { get; set; }

        /// <summary>
        /// Gets or sets the date of the job.
        /// </summary>
        public DateOnly? JobDate { get; set; }

        /// <summary>
        /// Gets or sets the time of the job (from post position start time).
        /// </summary>
        public TimeOnly? JobTime { get; set; }

        /// <summary>
        /// Gets or sets the officer name.
        /// </summary>
        [MaxLength(200, ErrorMessage = "OfficerName cannot exceed 200 characters")]
        public string? OfficerName { get; set; }

        /// <summary>
        /// Gets or sets the hourly or fixed rate.
        /// </summary>
        public decimal? Rate { get; set; }

        /// <summary>
        /// Gets or sets the alcohol special rate amount applied to this line when generated.
        /// </summary>
        public decimal? SpecialRate { get; set; }

        /// <summary>
        /// Base officer pay rate for this line (excludes <see cref="SpecialRate"/>).
        /// </summary>
        public decimal? OfficerRate => Rate.HasValue ? Rate.Value - (SpecialRate ?? 0m) : null;

        /// <summary>
        /// Gets or sets the number of hours worked.
        /// </summary>
        public decimal? Hours { get; set; }

        /// <summary>
        /// Gets or sets the vehicle fees amount for this line item.
        /// </summary>
        public decimal? VehicleFeeAmount { get; set; }

        /// <summary>
        /// Gets or sets the equipment fees amount for this line item.
        /// </summary>
        public decimal? EquipmentFeeAmount { get; set; }

        /// <summary>
        /// Gets or sets vehicle ownership from job close (City, Employee, or Other).
        /// </summary>
        public VehicleOwnedByType? VehicleOwnedBy { get; set; }

        /// <summary>
        /// Gets or sets who payments are payable directly to for this line item.
        /// </summary>
        public string? PayableDirectlyTo { get; set; }

        /// <summary>
        /// Gets or sets whether payments are payable to the officer for this line item.
        /// </summary>
        public bool IsPayableToOfficer { get; set; }

        /// <summary>
        /// Gets or sets the total amount for this line item.
        /// </summary>
        public decimal? TotalAmount { get; set; }

        /// <summary>
        /// Gets or sets the post length in minutes for this line item.
        /// </summary>
        public int? PostLength { get; set; }
        public Guid? PostPositionID { get; set; }

        /// <summary>
        /// Gets or sets the post/job identifier this line item belongs to.
        /// Used for distinct post count in PerPost fee calculations.
        /// </summary>
        public Guid? PostID { get; set; }

        /// <summary>
        /// True when the linked closed post position was billed using department minimum hours paid.
        /// </summary>
        public bool IsMinimumDue { get; set; }

        public decimal GetRateAmount()
        {
            return (Rate ?? 0m) * (Hours ?? 0m);
        }

        public decimal GetVehicleAmount()
        {
            return VehicleFeeAmount ?? 0m;
        }

        public decimal GetEquipmentAmount()
        {
            return EquipmentFeeAmount ?? 0m;
        }

        public decimal GetTotalAmount()
        {
            return GetRateAmount()
                   + GetVehicleAmount()
                   + GetEquipmentAmount();
        }

        private static bool IsQualifyingPremiumHoliday(HolidayDayDto holiday) =>
            holiday.IsActive == true
            && holiday.IsPremiumRate == true
            && holiday.PayPremiumForEntireShift == true;

        private static decimal GetPremiumHourlyRate(decimal basePayRate, EmployeeTypeDto? employeeType)
        {
            // Premium-day invoice rate must come from EmployeeType configuration,
            // so rate updates are reflected without requiring post edits.
            if (employeeType?.PremiumPayRate is > 0)
                return employeeType.PremiumPayRate.Value;

            if (employeeType?.HolidayPayRate is > 0)
                return employeeType.HolidayPayRate.Value;

            return basePayRate;
        }

        private static decimal ResolveInvoicePayRate(
            decimal basePayRate,
            DateOnly workDate,
            EmployeeTypeDto? employeeType,
            IReadOnlyDictionary<DateOnly, IReadOnlyList<HolidayDayDto>>? holidaysByDate)
        {
            if (holidaysByDate == null
                || !holidaysByDate.TryGetValue(workDate, out var holidaysOnDate))
            {
                return basePayRate;
            }

            var qualifyingHolidays = holidaysOnDate.Where(IsQualifyingPremiumHoliday).ToList();
            if (qualifyingHolidays.Count == 0)
                return basePayRate;

            var premiumRate = GetPremiumHourlyRate(basePayRate, employeeType);
            return premiumRate * qualifyingHolidays.Count;
        }
    }
}
