using StockAccounting.Checklist.Repositories;
using StockAccounting.Checklist.Services;

namespace StockAccounting.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task GetEmployeeDataListTestAsync()
        {
            // Arrange
            var repository = new GenericRepository();
            var inventoryService = new InventoryDataService(repository);
            var employeeService = new EmployeeDataService(repository);

            // Act
            var employees = await employeeService.GetEmployeeDataAsync();

            // Assert
            Assert.IsTrue(employees != null);
        }
    }
}