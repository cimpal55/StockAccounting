namespace StockAccounting.Core.Data.Resources
{
    public static class Columns
    {
        public static class StockTypes
        {
            public const string Id = "ID";
            public const string Name = "Name";
            public const string Abbreviature = "Abbreviature";
            public const string Created = "Created";

        }

        public static class Unit
        {
            public const string Id = "ID";
            public const string Name = "Name";
            public const string Created = "Created";
        }

        public static class Employee
        {
            public const string Id = "ID";
            public const string Name = "Name";
            public const string Surname = "Surname";
            public const string Code = "Code";
            public const string Email = "Email";
            public const string IsManager = "IsManager";
            public const string Created = "Created";
        }

        public static class ExternalData
        {
            public const string Id = "ID";
            public const string ItemNumber = "ItemNumber";
            public const string Barcode = "Barcode";
            public const string PluCode = "PluCode";
            public const string Quantity = "Quantity";
            public const string Name = "Name";
            public const string Unit = "Unit";
            public const string UnitId = "UnitID";
            public const string Created = "Created";
            public const string Updated = "Updated";
        }

        public static class InventoryData
        {
            public const string Id = "ID";
            public const string Name = "Name";
            public const string Employee1CheckerId = "Employee1CheckerID";
            public const string Employee2CheckerId = "Employee2CheckerID";
            public const string ScannedEmployeeId = "ScannedEmployeeID";
            public const string Status = "Status";
            public const string ManuallyAdded = "ManuallyAdded";
            public const string Created = "Created";
            public const string Updated = "Updated";
            public const string Finished = "Finished";
        }

        public static class DocumentData
        {
            public const string Id = "ID";
            public const string Employee1Id = "Employee1ID";
            public const string Employee2Id = "Employee2ID";
            public const string ManuallyAdded = "ManuallyAdded";
            public const string IsSynchronization = "IsSynchronization";
            public const string DocumentType = "DocumentType";
            public const string Created = "Created";
            public const string Updated = "Updated";
        }

        public static class ScannedData
        {
            public const string Id = "ID";
            public const string DocumentSerialNumber = "DocumentSerialNumber";
            public const string DocumentNumber = "DocumentNumber";
            public const string DocumentDataId = "DocumentDataID";
            public const string InventoryDataId = "InventoryDataID";
            public const string ExternalDataId = "ExternalDataID";
            public const string Quantity = "Quantity";
            public const string Created = "Created";
        }

        public static class ScannedInventoryData
        {
            public const string Id = "ID";
            public const string InventoryDataId = "InventoryDataID";
            public const string Name = "Name";
            public const string ItemNumber = "ItemNumber";
            public const string Barcode = "Barcode";
            public const string PluCode = "PluCode";
            public const string Unit = "Unit";
            public const string Quantity = "Quantity";
            public const string CheckedQuantity = "CheckedQuantity";
            public const string FinalQuantity = "FinalQuantity";
            public const string IsExternal = "IsExternal";
            public const string Created = "Created";
            public const string Updated = "Updated";
        }

        public static class StockEmployees
        {
            public const string Id = "ID";
            public const string DocumentSerialNumber = "DocumentSerialNumber";
            public const string DocumentNumber = "DocumentNumber";
            public const string StockDataId = "StockDataID";
            public const string EmployeeId = "EmployeeID";
            public const string StockTypeId = "StockTypeID";
            public const string Quantity = "Quantity";
            public const string Created = "Created";
            public const string LastSynchronization = "LastSynchronization";
        }

        public static class StockData
        {
            public const string Id = "ID";
            public const string ExternalDataId = "ExternalDataID";
            public const string Quantity = "Quantity";
            public const string LastSynchronization = "LastSynchronization";
            public const string Created = "Created";
        }

        public static class Toolkit
        {
            public const string Id = "ID";
            public const string Name = "Name";
            public const string Description = "Description";
            public const string TotalQuantity = "TotalQuantity";
            public const string Barcode = "Barcode";
            public const string IsDeleted = "IsDeleted";
            public const string Created = "Created";
            public const string Updated = "Updated";
        }

        public static class ToolkitExternal
        {
            public const string Id = "ID";
            public const string ToolkitId = "ToolkitId";
            public const string ExternalDataId = "ExternalDataId";
            public const string Quantity = "Quantity";
            public const string Created = "Created";
            public const string Updated = "Updated";
        }

        public static class ToolkitHistory
        {
            public const string Id = "ID";
            public const string ToolkitId = "ToolkitId";
            public const string EmployeeId = "EmployeeId";
            public const string Created = "Created";
        }
    }
}