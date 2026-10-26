using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>Builds department-equipment metadata from eager-loaded navigation properties.</summary>
    public static class ViewDetailsDepartmentEquipmentLookupBuilder
    {
        public static Dictionary<Guid, ViewDetailsDepartmentEquipmentMeta> FromLoadedEquipment(
            IEnumerable<RequestEquipment> requestEquipment,
            IEnumerable<PostEquipment> postEquipment)
        {
            var dict = new Dictionary<Guid, ViewDetailsDepartmentEquipmentMeta>();
            foreach (var de in requestEquipment.Select(re => re.DepartmentEquipment)
                         .Concat(postEquipment.Select(pe => pe.DepartmentEquipment))
                         .Where(de => de != null))
            {
                if (de!.DepartmentEquipmentID == Guid.Empty)
                    continue;
                dict[de.DepartmentEquipmentID] = ToMeta(de);
            }

            return dict;
        }

        private static ViewDetailsDepartmentEquipmentMeta ToMeta(DepartmentEquipment de)
        {
            var isVehicle = de.EquipmentType?.IsVehicle == true;
            var shortName = de.EquipmentType?.EquipementTypeShort?.Trim();
            var longName = de.EquipmentType?.EquipementTypeLong?.Trim();
            var name = !string.IsNullOrEmpty(shortName) ? shortName : longName;
            return new ViewDetailsDepartmentEquipmentMeta(
                isVehicle,
                name,
                de.Charge,
                de.MileageCharge,
                de.ChargeMileage,
                de.RequiresApproval,
                de.ChargedType);
        }
    }
}
