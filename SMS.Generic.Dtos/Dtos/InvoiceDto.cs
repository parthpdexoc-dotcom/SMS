using System.Globalization;
using System.Net;
using SMS.Generic.Common;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Dtos.Interfaces;
using SMS.Generic.Models.Constants;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for Invoice operations.
    /// </summary>
    public class InvoiceDto : BaseDto, IDepartmentScoped
    {
        public InvoiceDto()
        {
        }

        /// <summary>
        /// Creates an InvoiceDto with Bill To (request/requester billing) and optional From (department) fields.
        /// </summary>
        public InvoiceDto(
            Guid? requesterID,
            string? billingToName,
            string? billingToAddress,
            string? billingToPhoneNumber,
            string? billingToEmail,
            string? billingToContact,
            string? billingFromName = null,
            string? billingFromAddress = null,
            string? billingFromPhoneNumber = null,
            string? billingFromEmail = null,
            string? billingFromContact = null)
        {
            RequesterID = requesterID;
            BillingToName = billingToName;
            BillingToAddress = billingToAddress;
            BillingToPhoneNumber = billingToPhoneNumber;
            BillingToEmail = billingToEmail;
            BillingToContact = billingToContact;
            BillingFromName = billingFromName;
            BillingFromAddress = billingFromAddress;
            BillingFromPhoneNumber = billingFromPhoneNumber;
            BillingFromEmail = billingFromEmail;
            BillingFromContact = billingFromContact;
        }

        /// <summary>
        /// Gets or sets the unique identifier for the invoice.
        /// </summary>
        public Guid InvoicesID { get; set; }

        /// <summary>
        /// Gets or sets the invoice number.
        /// </summary>
        public string? InvoiceNo { get; set; }

        /// <summary>
        /// Gets or sets the date the invoice was created.
        /// </summary>
        public DateTime? DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the invoice due date.
        /// </summary>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Gets or sets the start date of the invoice's covered range.
        /// </summary>
        public DateOnly StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date of the invoice's covered range.
        /// </summary>
        public DateOnly EndDate { get; set; }

        /// <summary>
        /// Gets or sets the first job date worked in the invoiced range.
        /// </summary>
        public DateOnly? JobPositionStartDate { get; set; }

        /// <summary>
        /// Gets or sets the last job date worked in the invoiced range.
        /// </summary>
        public DateOnly? JobPositionEndDate { get; set; }

        /// <summary>
        /// Gets or sets the billing from name (department).
        /// </summary>
        public string? BillingFromName { get; set; }

        /// <summary>
        /// Gets or sets the billing from address (department).
        /// </summary>
        public string? BillingFromAddress { get; set; }

        /// <summary>
        /// Gets or sets the billing from phone number (department).
        /// </summary>
        public string? BillingFromPhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the billing from email (department).
        /// </summary>
        public string? BillingFromEmail { get; set; }

        /// <summary>
        /// Gets or sets the billing from contact (department).
        /// </summary>
        public string? BillingFromContact { get; set; }

        /// <summary>
        /// Gets or sets the billing to name (requester billing).
        /// </summary>
        public string? BillingToName { get; set; }

        /// <summary>
        /// Gets or sets the billing to address (requester billing).
        /// </summary>
        public string? BillingToAddress { get; set; }

        /// <summary>
        /// Gets or sets the billing to phone number (requester billing).
        /// </summary>
        public string? BillingToPhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the billing to email (requester billing).
        /// </summary>
        public string? BillingToEmail { get; set; }

        /// <summary>
        /// Gets or sets the billing to contact (requester billing).
        /// </summary>
        public string? BillingToContact { get; set; }

        /// <summary>
        /// Gets or sets the officer hourly total.
        /// </summary>
        public decimal? OfficerHourlyTotal { get; set; }

        /// <summary>
        /// Gets or sets the officer fees total.
        /// </summary>
        public decimal? OfficerFeesTotal { get; set; }

        /// <summary>
        /// Gets or sets the agency fees total.
        /// </summary>
        public decimal? AgencyFeesTotal { get; set; }

        /// <summary>
        /// Gets or sets the vehicle fees total.
        /// </summary>
        public decimal? VehicleFeesTotal { get; set; }

        /// <summary>
        /// Gets or sets the equipment fees total.
        /// </summary>
        public decimal? EquipmentFeesTotal { get; set; }

        /// <summary>
        /// Gets or sets the total amount.
        /// </summary>
        public decimal? Total { get; set; }

        /// <summary>
        /// Gets or sets the payments applied.
        /// </summary>
        public decimal? PaymentsApplied { get; set; }

        /// <summary>
        /// Gets or sets the balance due.
        /// </summary>
        public decimal? BalanceDue { get; set; }

        /// <summary>
        /// Gets or sets whether the invoice has been fully paid.
        /// </summary>
        public bool IsPaid { get; set; }

        /// <summary>
        /// Gets or sets the department identifier (foreign key to Department).
        /// Used for department-scoped filtering and display.
        /// </summary>
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the list of invoice detail line items (one invoice has many details).
        /// </summary>
        public List<InvoiceDetailDto>? InvoiceDetails { get; set; }

        /// <summary>
        /// Gets or sets the requester identifier (foreign key).
        /// </summary>
        public Guid? RequesterID { get; set; }

        /// <summary>
        /// Requester display name (populated when loaded with requester include).
        /// </summary>
        public string? RequesterName { get; set; }

        /// <summary>
        /// Gets or sets the related request identifier (foreign key).
        /// </summary>
        public Guid? RequestId { get; set; }

        /// <summary>
        /// Gets or sets the related request name for invoice messages/display.
        /// </summary>
        public string? RequestName { get; set; }

        /// <summary>
        /// Gets or sets the post length (e.g. in minutes) for the request/invoice context.
        /// </summary>
        public int? PostLength { get; set; }

        /// <summary>
        /// Gets or sets who payments are payable directly to (from department invoice config).
        /// </summary>
        public string? PayableDirectlyTo { get; set; }

        /// <summary>
        /// Gets or sets whether payments are payable to the officer (from department invoice config).
        /// </summary>
        public bool IsPayableToOfficer { get; set; }

        /// <summary>
        /// Merges another invoice's totals and optionally line items into this invoice.
        /// </summary>
        /// <param name="other">The invoice to merge from.</param>
        /// <param name="includeLineItems">When true, appends <see cref="InvoiceDetails"/> from <paramref name="other"/>.</param>
        public void MergeFrom(InvoiceDto other, bool includeLineItems = true)
        {
            ArgumentNullException.ThrowIfNull(other);

            if (includeLineItems && other.InvoiceDetails?.Count > 0)
            {
                InvoiceDetails ??= new List<InvoiceDetailDto>();
                InvoiceDetails.AddRange(other.InvoiceDetails);
            }

            OfficerHourlyTotal = (OfficerHourlyTotal ?? 0m) + (other.OfficerHourlyTotal ?? 0m);
            OfficerFeesTotal = (OfficerFeesTotal ?? 0m) + (other.OfficerFeesTotal ?? 0m);
            AgencyFeesTotal = (AgencyFeesTotal ?? 0m) + (other.AgencyFeesTotal ?? 0m);
            VehicleFeesTotal = (VehicleFeesTotal ?? 0m) + (other.VehicleFeesTotal ?? 0m);
            EquipmentFeesTotal = (EquipmentFeesTotal ?? 0m) + (other.EquipmentFeesTotal ?? 0m);
            Total = (Total ?? 0m) + (other.Total ?? 0m);

            if (other.PaymentsApplied.HasValue)
                PaymentsApplied = (PaymentsApplied ?? 0m) + other.PaymentsApplied.Value;

            BalanceDue = Total;
        }

        /// <summary>
        /// Computes and sets all fee totals and balance from <see cref="InvoiceDetails"/> and the supplied department fee totals.
        /// </summary>
        /// <param name="officerFeesTotal">Pre-calculated officer fees total (from department fees).</param>
        /// <param name="agencyFeesTotal">Pre-calculated agency fees total (from department fees).</param>
        /// <param name="updateCreatedAndDueDates">When true, sets <see cref="DateCreated"/> and <see cref="DueDate"/> (e.g. new invoice draft). When false, leaves them unchanged.</param>
        /// <param name="subtractPaymentsFromBalance">When true, sets <see cref="BalanceDue"/> to <c>Total - paymentsApplied</c>.</param>
        /// <param name="paymentsApplied">Payments to subtract when <paramref name="subtractPaymentsFromBalance"/> is true; null treated as zero.</param>
        public void ComputeAndSetTotals(decimal officerFeesTotal,decimal agencyFeesTotal,bool updateCreatedAndDueDates = true,bool subtractPaymentsFromBalance = false,decimal? paymentsApplied = null)
        {
            var details = InvoiceDetails ?? new List<InvoiceDetailDto>();
            var officerHourlyTotal = details.Sum(d => (d.Rate ?? 0m) * (d.Hours ?? 0m));
            var vehicleFeesTotal = details.Sum(d => d.VehicleFeeAmount ?? 0m);
            var equipmentFeesTotal = details.Sum(d => d.EquipmentFeeAmount ?? 0m);

            OfficerHourlyTotal = officerHourlyTotal;
            OfficerFeesTotal = officerFeesTotal;
            AgencyFeesTotal = agencyFeesTotal;
            VehicleFeesTotal = vehicleFeesTotal;
            EquipmentFeesTotal = equipmentFeesTotal;

            if (updateCreatedAndDueDates)
            {
                DateCreated = DateTime.UtcNow;
                DueDate = DateCreated?.AddDays(30);
            }

            Total = officerHourlyTotal + officerFeesTotal + agencyFeesTotal + vehicleFeesTotal + equipmentFeesTotal;

            if (subtractPaymentsFromBalance)
                BalanceDue = (Total ?? 0m) - (paymentsApplied ?? 0m);
            else
                BalanceDue = Total;
        }

        /// <summary>
        /// Filters invoices by paid status.
        /// </summary>
        public static List<InvoiceDto> FilterInvoicesByPaidStatus(IEnumerable<InvoiceDto> invoices, bool isPaid) =>invoices.Where(i => i.IsPaid == isPaid).ToList();

        /// <summary>Sums <see cref="Total"/> across invoices.</summary>
        public static decimal SumTotal(IEnumerable<InvoiceDto>? invoices) =>Sum(invoices, i => i.Total ?? 0m);

        /// <summary>Sums <see cref="PaymentsApplied"/> across invoices.</summary>
        public static decimal SumPaymentsApplied(IEnumerable<InvoiceDto>? invoices) =>Sum(invoices, i => i.PaymentsApplied ?? 0m);

        /// <summary>Sums <see cref="BalanceDue"/> across invoices.</summary>
        public static decimal SumBalanceDue(IEnumerable<InvoiceDto>? invoices) =>Sum(invoices, i => i.BalanceDue ?? 0m);

        /// <summary>
        /// Applies manual edits to invoice detail rows in memory (does not persist).
        /// </summary>
        public static List<InvoiceDetailDto> ApplyDetailChanges(
            IReadOnlyList<InvoiceDetailDto> details,
            IEnumerable<InvoiceDetailDto> edits)
        {
            var result = details.ToList();

            foreach (var edit in edits)
            {
                if (edit.InvoiceDetailID == Guid.Empty)
                    continue;

                var detail = result.FirstOrDefault(d => d.InvoiceDetailID == edit.InvoiceDetailID);
                if (detail == null)
                    continue;

                detail.Rate = edit.Rate;
                detail.Hours = edit.Hours;
                detail.VehicleFeeAmount = CurrencyRoundHelper.RoundTwo(edit.VehicleFeeAmount);
                detail.EquipmentFeeAmount = CurrencyRoundHelper.RoundTwo(edit.EquipmentFeeAmount);
            }

            return result;
        }

        /// <summary>
        /// Validates recalculated totals from detail lines using <see cref="ApplyRecalculatedTotal"/> (validate-only).
        /// </summary>
        public void ValidateRecalculatedTotalFromDetails(
            IReadOnlyList<InvoiceDetailDto> details,
            decimal officerFeesTotal,
            decimal agencyFeesTotal,
            Guid? closedPostPositionId = null)
        {
            var projected = new InvoiceDto
            {
                InvoiceDetails = details.ToList(),
            };
            projected.ComputeAndSetTotals(officerFeesTotal, agencyFeesTotal, updateCreatedAndDueDates: false);
            var newTotal = projected.Total ?? 0m;

            ApplyRecalculatedTotal(newTotal, validateOnly: true, closedPostPositionId);
        }

        /// <summary>
        /// Applies a recalculated invoice total: rejects decreases below <see cref="PaymentsApplied"/> (HTTP 409),
        /// then updates <see cref="Total"/>, <see cref="BalanceDue"/>, and <see cref="IsPaid"/>.
        /// Pass <paramref name="validateOnly"/> to check before persisting line-item changes.
        /// </summary>
        public void ApplyRecalculatedTotal(decimal newTotal, bool validateOnly = false, Guid? closedPostPositionId = null)
        {
            var paid = PaymentsApplied ?? 0m;

            if (newTotal < 0m)
            {
                var invoiceNo = !string.IsNullOrWhiteSpace(InvoiceNo)
                    ? InvoiceNo.Trim()
                        : "unknown";

                throw new WebApiException(
                    HttpStatusCode.BadRequest,
                    ((int)HttpStatusCode.BadRequest).ToString(),
                    ModelValidationErrorCodes.GetErrorMessage(
                        ModelValidationErrorCodes.INVOICE_UNCLOSE_ZERO_TOTAL_REQUIRES_DELETE,
                        invoiceNo));
            }

            if (paid > newTotal)
            {
                var refundRequired = paid - newTotal;
                var currencyCulture = CultureInfo.GetCultureInfo("en-US");
                throw new WebApiException(
                    HttpStatusCode.Conflict,
                    ((int)HttpStatusCode.Conflict).ToString(),
                    ModelValidationErrorCodes.GetErrorMessage(
                        ModelValidationErrorCodes.INVOICE_UNCLOSE_REQUIRES_REFUND,
                        paid.ToString("C2", currencyCulture),
                        newTotal.ToString("C2", currencyCulture),
                        refundRequired.ToString("C2", currencyCulture)),
                    null,
                    refundRequired,
                    InvoicesID != Guid.Empty ? InvoicesID : null,
                    closedPostPositionId is Guid id && id != Guid.Empty ? id : null);
            }

            if (validateOnly)
                return;

            Total = newTotal;
            BalanceDue = Math.Max(0m, newTotal - paid);
            IsPaid = BalanceDue == 0m;
        }
    }
}
