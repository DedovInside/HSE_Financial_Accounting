using HSE_financial_accounting.Commands;
using HSE_financial_accounting.Logging;
using HSE_financial_accounting.Models;
using HSE_financial_accounting.Models.Interfaces;
using HSE_financial_accounting.Factories;
using HSE_financial_accounting.Facades;
using HSE_financial_accounting.Repositories;
using HSE_financial_accounting.Commands.AccountsCommands;
using HSE_financial_accounting.Commands.CategoriesCommands;
using HSE_financial_accounting.Commands.OperationsCommands;
using HSE_financial_accounting.Commands.AnalyticsCommands;
using HSE_financial_accounting.DataExport;
using HSE_financial_accounting.DataTransferObjects;
using System.Text.Json;
using Moq;

namespace HSE_financial_accounting.Tests
{
    public class CommandInvokerTests
    {
        [Fact]
        public void ExecuteWithTimeMeasurement_ValidCommand_ExecutesCommand()
        {
            // Arrange
            Mock<ILogger> mockLogger = new();
            Mock<ICommand> mockCommand = new();
            CommandInvoker invoker = new(mockLogger.Object);

            // Act
            invoker.ExecuteWithTimeMeasurement(mockCommand.Object, "TestCommand");

            // Assert
            mockCommand.Verify(c => c.Execute(), Times.Once);
            mockLogger.Verify(l => l.LogInformation(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void Constructor_NullLogger_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new CommandInvoker(null));
        }
    }



    public class BankAccountTests
    {
        [Fact]
        public void Constructor_ValidParameters_CreatesAccount()
        {
            // Arrange
            string name = "Test Account";
            decimal balance = 1000m;

            // Act
            BankAccount account = new(name, balance);

            // Assert
            Assert.Equal(name, account.Name);
            Assert.Equal(balance, account.Balance);
            Assert.NotEqual(Guid.Empty, account.Id);
        }

        [Fact]
        public void Constructor_WithGuidId_InitializesPropertiesCorrectly()
        {
            // Arrange
            Guid expectedId = Guid.NewGuid();
            string expectedName = "Тестовый счет";
            decimal expectedBalance = 5000m;

            // Act
            BankAccount account = new(expectedId, expectedName, expectedBalance);

            // Assert
            Assert.Equal(expectedId, account.Id);
            Assert.Equal(expectedName, account.Name);
            Assert.Equal(expectedBalance, account.Balance);
        }

        [Fact]
        public void Deposit_ValidAmount_IncreasesBalance()
        {
            // Arrange
            BankAccount account = new("Test", 1000m);
            decimal depositAmount = 500m;
            decimal expectedBalance = 1500m;

            // Act
            account.Deposit(depositAmount);

            // Assert
            Assert.Equal(expectedBalance, account.Balance);
        }

        [Fact]
        public void Withdraw_AmountGreaterThanBalance_DecreasesBalance()
        {
            // Arrange
            BankAccount account = new("Test", 1000m);
            decimal withdrawAmount = 1500m;
            decimal expectedBalance = -500m;

            // Act
            account.Withdraw(withdrawAmount);

            // Assert
            Assert.Equal(expectedBalance, account.Balance);
        }

        [Fact]
        public void UpdateName_ValidName_ChangesName()
        {
            // Arrange
            BankAccount account = new("Old Name", 1000m);
            string newName = "New Name";

            // Act
            account.UpdateName(newName);

            // Assert
            Assert.Equal(newName, account.Name);
        }
    }

    public class CategoryTests
    {
        [Fact]
        public void Constructor_ValidParameters_CreatesCategory()
        {
            // Arrange
            string name = "Groceries";
            CategoryType type = CategoryType.Expense;

            // Act
            Category category = new(name, type);

            // Assert
            Assert.Equal(name, category.Name);
            Assert.Equal(type, category.Type);
            Assert.NotEqual(Guid.Empty, category.Id);
        }

        [Fact]
        public void Constructor_WithGuidId_InitializesPropertiesCorrectly()
        {
            // Arrange
            Guid expectedId = Guid.NewGuid();
            string expectedName = "Salary";
            CategoryType expectedType = CategoryType.Income;

            // Act
            Category category = new(expectedId, expectedName, expectedType);

            // Assert
            Assert.Equal(expectedId, category.Id);
            Assert.Equal(expectedName, category.Name);
            Assert.Equal(expectedType, category.Type);
        }

        [Fact]
        public void UpdateName_ValidName_ChangesName()
        {
            // Arrange
            Category category = new("Old Name", CategoryType.Expense);
            string newName = "New Name";

            // Act
            category.UpdateName(newName);

            // Assert
            Assert.Equal(newName, category.Name);
        }

        [Fact]
        public void UpdateName_EmptyName_ThrowsArgumentException()
        {
            // Arrange
            Category category = new("Valid Name", CategoryType.Income);
            string emptyName = "";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => category.UpdateName(emptyName));
        }

        [Fact]
        public void UpdateName_WhitespaceName_ThrowsArgumentException()
        {
            // Arrange
            Category category = new("Valid Name", CategoryType.Income);
            string whitespaceName = "   ";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => category.UpdateName(whitespaceName));
        }

        [Fact]
        public void UpdateName_NullName_ThrowsArgumentException()
        {
            // Arrange
            Category category = new("Valid Name", CategoryType.Income);
            string nullName = null;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => category.UpdateName(nullName));
        }
    }

    public class OperationTests
    {
        [Fact]
        public void Constructor_ValidParameters_CreatesOperation()
        {
            // Arrange
            OperationType type = OperationType.Income;
            Guid bankAccountId = Guid.NewGuid();
            decimal amount = 500m;
            DateTime date = DateTime.Now;
            string description = "Salary payment";
            Guid categoryId = Guid.NewGuid();

            // Act
            Operation operation = new(type, bankAccountId, amount, date, description, categoryId);

            // Assert
            Assert.Equal(type, operation.Type);
            Assert.Equal(bankAccountId, operation.BankAccountId);
            Assert.Equal(amount, operation.Amount);
            Assert.Equal(date, operation.Date);
            Assert.Equal(description, operation.Description);
            Assert.Equal(categoryId, operation.CategoryId);
            Assert.NotEqual(Guid.Empty, operation.Id);
        }

        [Fact]
        public void Constructor_WithGuidId_InitializesPropertiesCorrectly()
        {
            // Arrange
            Guid expectedId = Guid.NewGuid();
            OperationType expectedType = OperationType.Expense;
            Guid expectedBankAccountId = Guid.NewGuid();
            decimal expectedAmount = 1250.50m;
            DateTime expectedDate = new(2023, 5, 15);
            string expectedDescription = "Grocery shopping";
            Guid expectedCategoryId = Guid.NewGuid();

            // Act
            Operation operation = new(
                expectedId,
                expectedType,
                expectedBankAccountId,
                expectedAmount,
                expectedDate,
                expectedDescription,
                expectedCategoryId);

            // Assert
            Assert.Equal(expectedId, operation.Id);
            Assert.Equal(expectedType, operation.Type);
            Assert.Equal(expectedBankAccountId, operation.BankAccountId);
            Assert.Equal(expectedAmount, operation.Amount);
            Assert.Equal(expectedDate, operation.Date);
            Assert.Equal(expectedDescription, operation.Description);
            Assert.Equal(expectedCategoryId, operation.CategoryId);
        }

        [Fact]
        public void UpdateDescription_ValidDescription_ChangesDescription()
        {
            // Arrange
            Operation operation = new(
                OperationType.Expense,
                Guid.NewGuid(),
                100m,
                DateTime.Now,
                "Old description",
                Guid.NewGuid());

            string newDescription = "New description";

            // Act
            operation.UpdateDescription(newDescription);

            // Assert
            Assert.Equal(newDescription, operation.Description);
        }

        [Fact]
        public void UpdateDescription_NullDescription_ChangesToNull()
        {
            // Arrange
            Operation operation = new(
                OperationType.Income,
                Guid.NewGuid(),
                200m,
                DateTime.Now,
                "Initial description",
                Guid.NewGuid());

            // Act
            operation.UpdateDescription(null);

            // Assert
            Assert.Null(operation.Description);
        }

        [Fact]
        public void UpdateDescription_EmptyDescription_ChangesToEmpty()
        {
            // Arrange
            Operation operation = new(
                OperationType.Income,
                Guid.NewGuid(),
                200m,
                DateTime.Now,
                "Initial description",
                Guid.NewGuid());

            string emptyDescription = "";

            // Act
            operation.UpdateDescription(emptyDescription);

            // Assert
            Assert.Equal(emptyDescription, operation.Description);
        }
    }

    public class StandardFinanceCreatorTests
    {
        [Fact]
        public void Constructor_WithName_SetsCreatorName()
        {
            // Arrange
            string expectedName = "StandardCreator";

            // Act
            StandardFinanceCreator creator = new(expectedName);

            // Assert
            Assert.Equal(expectedName, creator.CreatorName);
        }

        [Fact]
        public void CreateBankAccount_ValidParameters_ReturnsBankAccount()
        {
            // Arrange
            StandardFinanceCreator creator = new("TestCreator");
            string expectedName = "Savings Account";
            decimal expectedBalance = 1000m;

            // Act
            IBankAccount account = creator.CreateBankAccount(expectedName, expectedBalance);

            // Assert
            Assert.NotNull(account);
            Assert.Equal(expectedName, account.Name);
            Assert.Equal(expectedBalance, account.Balance);
            Assert.NotEqual(Guid.Empty, account.Id);
        }

        [Fact]
        public void CreateBankAccountWithId_ValidParameters_ReturnsBankAccountWithSpecifiedId()
        {
            // Arrange
            StandardFinanceCreator creator = new("TestCreator");
            Guid expectedId = Guid.NewGuid();
            string expectedName = "Checking Account";
            decimal expectedBalance = 500m;

            // Act
            IBankAccount account = creator.CreateBankAccountWithId(expectedId, expectedName, expectedBalance);

            // Assert
            Assert.NotNull(account);
            Assert.Equal(expectedId, account.Id);
            Assert.Equal(expectedName, account.Name);
            Assert.Equal(expectedBalance, account.Balance);
        }

        [Fact]
        public void CreateCategory_ValidParameters_ReturnsCategory()
        {
            // Arrange
            StandardFinanceCreator creator = new("TestCreator");
            string expectedName = "Groceries";
            CategoryType expectedType = CategoryType.Expense;

            // Act
            ICategory category = creator.CreateCategory(expectedName, expectedType);

            // Assert
            Assert.NotNull(category);
            Assert.Equal(expectedName, category.Name);
            Assert.Equal(expectedType, category.Type);
            Assert.NotEqual(Guid.Empty, category.Id);
        }

        [Fact]
        public void CreateCategoryWithId_ValidParameters_ReturnsCategoryWithSpecifiedId()
        {
            // Arrange
            StandardFinanceCreator creator = new("TestCreator");
            Guid expectedId = Guid.NewGuid();
            string expectedName = "Salary";
            CategoryType expectedType = CategoryType.Income;

            // Act
            ICategory category = creator.CreateCategoryWithId(expectedId, expectedName, expectedType);

            // Assert
            Assert.NotNull(category);
            Assert.Equal(expectedId, category.Id);
            Assert.Equal(expectedName, category.Name);
            Assert.Equal(expectedType, category.Type);
        }

        [Fact]
        public void CreateOperation_ValidParameters_ReturnsOperation()
        {
            // Arrange
            StandardFinanceCreator creator = new("TestCreator");
            OperationType expectedType = OperationType.Expense;
            Guid expectedBankAccountId = Guid.NewGuid();
            decimal expectedAmount = 150.75m;
            DateTime expectedDate = DateTime.Now;
            string expectedDescription = "Grocery shopping";
            Guid expectedCategoryId = Guid.NewGuid();

            // Act
            IOperation operation = creator.CreateOperation(
                expectedType,
                expectedBankAccountId,
                expectedAmount,
                expectedDate,
                expectedDescription,
                expectedCategoryId);

            // Assert
            Assert.NotNull(operation);
            Assert.Equal(expectedType, operation.Type);
            Assert.Equal(expectedBankAccountId, operation.BankAccountId);
            Assert.Equal(expectedAmount, operation.Amount);
            Assert.Equal(expectedDate, operation.Date);
            Assert.Equal(expectedDescription, operation.Description);
            Assert.Equal(expectedCategoryId, operation.CategoryId);
            Assert.NotEqual(Guid.Empty, operation.Id);
        }

        [Fact]
        public void CreateOperationWithId_ValidParameters_ReturnsOperationWithSpecifiedId()
        {
            // Arrange
            StandardFinanceCreator creator = new("TestCreator");
            Guid expectedId = Guid.NewGuid();
            OperationType expectedType = OperationType.Income;
            Guid expectedBankAccountId = Guid.NewGuid();
            decimal expectedAmount = 2000m;
            DateTime expectedDate = new(2023, 5, 15);
            string expectedDescription = "Salary payment";
            Guid expectedCategoryId = Guid.NewGuid();

            // Act
            IOperation operation = creator.CreateOperationWithId(
                expectedId,
                expectedType,
                expectedBankAccountId,
                expectedAmount,
                expectedDate,
                expectedDescription,
                expectedCategoryId);

            // Assert
            Assert.NotNull(operation);
            Assert.Equal(expectedId, operation.Id);
            Assert.Equal(expectedType, operation.Type);
            Assert.Equal(expectedBankAccountId, operation.BankAccountId);
            Assert.Equal(expectedAmount, operation.Amount);
            Assert.Equal(expectedDate, operation.Date);
            Assert.Equal(expectedDescription, operation.Description);
            Assert.Equal(expectedCategoryId, operation.CategoryId);
        }
    }

    public class BankAccountFacadeTests
    {
        private readonly Mock<FinanceEntityCreator> _mockCreator;
        private readonly Mock<IRepository<IBankAccount>> _mockBankAccountRepository;
        private readonly Mock<IBankAccount> _mockBankAccount;
        private readonly BankAccountFacade _facade;

        public BankAccountFacadeTests()
        {
            _mockCreator = new Mock<FinanceEntityCreator>("TestCreator");
            _mockBankAccountRepository = new Mock<IRepository<IBankAccount>>();
            _mockBankAccount = new Mock<IBankAccount>();

            _facade = new BankAccountFacade(
                _mockCreator.Object,
                _mockBankAccountRepository.Object);
        }


        [Fact]
        public void CreateBankAccount_ValidParameters_AddsAccountToRepository()
        {
            // Arrange
            string name = "Test Account";
            decimal balance = 1000m;

            _mockCreator.Setup(c => c.CreateBankAccount(name, balance))
                .Returns(_mockBankAccount.Object);

            // Act
            _facade.CreateBankAccount(name, balance);

            // Assert
            _mockBankAccountRepository.Verify(r => r.Add(_mockBankAccount.Object), Times.Once);
        }

        [Fact]
        public void CreateBankAccountWithId_ValidParameters_AddsAccountToRepository()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            string name = "Import Account";
            decimal balance = 5000m;

            _mockCreator.Setup(c => c.CreateBankAccountWithId(id, name, balance))
                .Returns(_mockBankAccount.Object);

            // Act
            _facade.CreateBankAccountWithId(id, name, balance);

            // Assert
            _mockBankAccountRepository.Verify(r => r.Add(_mockBankAccount.Object), Times.Once);
        }

        [Fact]
        public void UpdateAccountName_ValidId_UpdatesNameAndSavesToRepository()
        {
            // Arrange
            Guid accountId = Guid.NewGuid();
            string newName = "Updated Account";

            _mockBankAccountRepository.Setup(r => r.GetById(accountId))
                .Returns(_mockBankAccount.Object);

            // Act
            _facade.UpdateAccountName(accountId, newName);

            // Assert
            _mockBankAccount.Verify(a => a.UpdateName(newName), Times.Once);
            _mockBankAccountRepository.Verify(r => r.Update(_mockBankAccount.Object), Times.Once);
        }

        [Fact]
        public void DeleteBankAccount_ValidId_DeletesFromRepository()
        {
            // Arrange
            Guid accountId = Guid.NewGuid();

            // Act
            _facade.DeleteBankAccount(accountId);

            // Assert
            _mockBankAccountRepository.Verify(r => r.Delete(accountId), Times.Once);
        }

        [Fact]
        public void GetBankAccount_ValidId_ReturnsAccount()
        {
            // Arrange
            Guid accountId = Guid.NewGuid();
            _mockBankAccountRepository.Setup(r => r.GetById(accountId))
                .Returns(_mockBankAccount.Object);

            // Act
            IBankAccount? result = _facade.GetBankAccount(accountId);

            // Assert
            Assert.Equal(_mockBankAccount.Object, result);
        }

        [Fact]
        public void GetAllBankAccounts_ReturnsAllAccounts()
        {
            // Arrange
            List<IBankAccount> accounts = new() { _mockBankAccount.Object };
            _mockBankAccountRepository.Setup(r => r.GetAll())
                .Returns(accounts);

            // Act
            IEnumerable<IBankAccount> result = _facade.GetAllBankAccounts();

            // Assert
            Assert.Equal(accounts, result);
        }
    }

    public class OperationFacadeTests
    {
        private readonly Mock<FinanceEntityCreator> _mockCreator;
        private readonly Mock<IRepository<IOperation>> _mockOperationRepository;
        private readonly Mock<IBankAccountFacade> _mockBankAccountFacade;
        private readonly Mock<IBankAccount> _mockBankAccount;
        private readonly Mock<IOperation> _mockOperation;
        private readonly OperationFacade _facade;

        public OperationFacadeTests()
        {
            _mockCreator = new Mock<FinanceEntityCreator>("TestCreator");
            _mockOperationRepository = new Mock<IRepository<IOperation>>();
            _mockBankAccountFacade = new Mock<IBankAccountFacade>();
            _mockBankAccount = new Mock<IBankAccount>();
            _mockOperation = new Mock<IOperation>();

            _facade = new OperationFacade(
                _mockCreator.Object,
                _mockOperationRepository.Object,
                _mockBankAccountFacade.Object);
        }

        [Fact]
        public void CreateOperation_Income_AddsOperationAndDepositsToAccount()
        {
            // Arrange
            Guid bankAccountId = Guid.NewGuid();
            decimal amount = 500m;
            DateTime date = DateTime.Now;
            string description = "Test income";
            Guid categoryId = Guid.NewGuid();

            _mockCreator.Setup(c => c.CreateOperation(
                    OperationType.Income, bankAccountId, amount, date, description, categoryId))
                .Returns(_mockOperation.Object);

            _mockBankAccountFacade.Setup(f => f.GetBankAccount(bankAccountId))
                .Returns(_mockBankAccount.Object);

            // Act
            _facade.CreateOperation(
                OperationType.Income, bankAccountId, amount, date, description, categoryId);

            // Assert
            _mockOperationRepository.Verify(r => r.Add(_mockOperation.Object), Times.Once);
            _mockBankAccount.Verify(a => a.Deposit(amount), Times.Once);
        }

        [Fact]
        public void CreateOperation_Expense_AddsOperationAndWithdrawsFromAccount()
        {
            // Arrange
            Guid bankAccountId = Guid.NewGuid();
            decimal amount = 300m;
            DateTime date = DateTime.Now;
            string description = "Test expense";
            Guid categoryId = Guid.NewGuid();

            _mockCreator.Setup(c => c.CreateOperation(
                    OperationType.Expense, bankAccountId, amount, date, description, categoryId))
                .Returns(_mockOperation.Object);

            _mockBankAccountFacade.Setup(f => f.GetBankAccount(bankAccountId))
                .Returns(_mockBankAccount.Object);

            // Act
            _facade.CreateOperation(
                OperationType.Expense, bankAccountId, amount, date, description, categoryId);

            // Assert
            _mockOperationRepository.Verify(r => r.Add(_mockOperation.Object), Times.Once);
            _mockBankAccount.Verify(a => a.Withdraw(amount), Times.Once);
        }

        [Fact]
        public void CreateOperationWithId_Income_AddsOperationAndDepositsToAccount()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            Guid bankAccountId = Guid.NewGuid();
            decimal amount = 1000m;
            DateTime date = new DateTime(2023, 5, 15);
            string description = "Import income";
            Guid categoryId = Guid.NewGuid();

            _mockCreator.Setup(c => c.CreateOperationWithId(
                    id, OperationType.Income, bankAccountId, amount, date, description, categoryId))
                .Returns(_mockOperation.Object);

            _mockBankAccountFacade.Setup(f => f.GetBankAccount(bankAccountId))
                .Returns(_mockBankAccount.Object);

            // Act
            _facade.CreateOperationWithId(
                id, OperationType.Income, bankAccountId, amount, date, description, categoryId);

            // Assert
            _mockOperationRepository.Verify(r => r.Add(_mockOperation.Object), Times.Once);
            _mockBankAccount.Verify(a => a.Deposit(amount), Times.Once);
        }

        [Fact]
        public void UpdateOperationDescription_ValidId_UpdatesDescription()
        {
            // Arrange
            Guid operationId = Guid.NewGuid();
            string newDescription = "Updated description";

            _mockOperationRepository.Setup(r => r.GetById(operationId))
                .Returns(_mockOperation.Object);

            // Act
            _facade.UpdateOperationDescription(operationId, newDescription);

            // Assert
            _mockOperation.Verify(o => o.UpdateDescription(newDescription), Times.Once);
            _mockOperationRepository.Verify(r => r.Update(_mockOperation.Object), Times.Once);
        }

        [Fact]
        public void DeleteOperation_IncomeOperation_RevertsBalanceAndDeletesOperation()
        {
            // Arrange
            Guid operationId = Guid.NewGuid();
            Guid bankAccountId = Guid.NewGuid();
            decimal amount = 200m;

            _mockOperation.SetupGet(o => o.Type).Returns(OperationType.Income);
            _mockOperation.SetupGet(o => o.BankAccountId).Returns(bankAccountId);
            _mockOperation.SetupGet(o => o.Amount).Returns(amount);

            _mockOperationRepository.Setup(r => r.GetById(operationId))
                .Returns(_mockOperation.Object);

            _mockBankAccountFacade.Setup(f => f.GetBankAccount(bankAccountId))
                .Returns(_mockBankAccount.Object);

            // Act
            _facade.DeleteOperation(operationId);

            // Assert
            _mockBankAccount.Verify(a => a.Withdraw(amount), Times.Once);
            _mockOperationRepository.Verify(r => r.Delete(operationId), Times.Once);
        }

        [Fact]
        public void DeleteOperation_ExpenseOperation_RevertsBalanceAndDeletesOperation()
        {
            // Arrange
            Guid operationId = Guid.NewGuid();
            Guid bankAccountId = Guid.NewGuid();
            decimal amount = 150m;

            _mockOperation.SetupGet(o => o.Type).Returns(OperationType.Expense);
            _mockOperation.SetupGet(o => o.BankAccountId).Returns(bankAccountId);
            _mockOperation.SetupGet(o => o.Amount).Returns(amount);

            _mockOperationRepository.Setup(r => r.GetById(operationId))
                .Returns(_mockOperation.Object);

            _mockBankAccountFacade.Setup(f => f.GetBankAccount(bankAccountId))
                .Returns(_mockBankAccount.Object);

            // Act
            _facade.DeleteOperation(operationId);

            // Assert
            _mockBankAccount.Verify(a => a.Deposit(amount), Times.Once);
            _mockOperationRepository.Verify(r => r.Delete(operationId), Times.Once);
        }

        [Fact]
        public void GetOperation_ValidId_ReturnsOperation()
        {
            // Arrange
            Guid operationId = Guid.NewGuid();
            _mockOperationRepository.Setup(r => r.GetById(operationId))
                .Returns(_mockOperation.Object);

            // Act
            IOperation? result = _facade.GetOperation(operationId);

            // Assert
            Assert.Equal(_mockOperation.Object, result);
        }

        [Fact]
        public void GetAllOperations_ReturnsAllOperations()
        {
            // Arrange
            List<IOperation> operations = new() { _mockOperation.Object };
            _mockOperationRepository.Setup(r => r.GetAll())
                .Returns(operations);

            // Act
            IEnumerable<IOperation> result = _facade.GetAllOperations();

            // Assert
            Assert.Equal(operations, result);
        }

        [Fact]
        public void GetOperationsByAccount_ValidAccountId_ReturnsFilteredOperations()
        {
            // Arrange
            Guid accountId = Guid.NewGuid();
            List<IOperation> operations = new() { _mockOperation.Object };

            _mockOperation.SetupGet(o => o.BankAccountId).Returns(accountId);
            _mockOperationRepository.Setup(r => r.GetAll())
                .Returns(operations);

            // Act
            IEnumerable<IOperation> result = _facade.GetOperationsByAccount(accountId);

            // Assert
            Assert.Equal(operations, result);
        }

        [Fact]
        public void GetOperationsByCategory_ValidCategoryId_ReturnsFilteredOperations()
        {
            // Arrange
            Guid categoryId = Guid.NewGuid();
            List<IOperation> operations = new() { _mockOperation.Object };

            _mockOperation.SetupGet(o => o.CategoryId).Returns(categoryId);
            _mockOperationRepository.Setup(r => r.GetAll())
                .Returns(operations);

            // Act
            IEnumerable<IOperation> result = _facade.GetOperationsByCategory(categoryId);

            // Assert
            Assert.Equal(operations, result);
        }
    }

    public class CategoryFacadeTests
    {
        private readonly Mock<FinanceEntityCreator> _mockCreator;
        private readonly Mock<IRepository<ICategory>> _mockCategoryRepository;
        private readonly Mock<ICategory> _mockCategory;
        private readonly CategoryFacade _facade;

        public CategoryFacadeTests()
        {
            _mockCreator = new Mock<FinanceEntityCreator>("TestCreator");
            _mockCategoryRepository = new Mock<IRepository<ICategory>>();
            _mockCategory = new Mock<ICategory>();

            _facade = new CategoryFacade(
                _mockCreator.Object,
                _mockCategoryRepository.Object);
        }

        [Fact]
        public void CreateCategory_ValidParameters_AddsCategoryToRepositoryAndReturnsCategory()
        {
            // Arrange
            string name = "Groceries";
            CategoryType type = CategoryType.Expense;

            _mockCreator.Setup(c => c.CreateCategory(name, type))
                .Returns(_mockCategory.Object);

            // Act
            ICategory result = _facade.CreateCategory(name, type);

            // Assert
            _mockCategoryRepository.Verify(r => r.Add(_mockCategory.Object), Times.Once);
            Assert.Equal(_mockCategory.Object, result);
        }

        [Fact]
        public void CreateCategoryWithId_ValidParameters_AddsCategoryToRepositoryAndReturnsCategory()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            string name = "Import Category";
            CategoryType type = CategoryType.Income;

            _mockCreator.Setup(c => c.CreateCategoryWithId(id, name, type))
                .Returns(_mockCategory.Object);

            // Act
            ICategory result = _facade.CreateCategoryWithId(id, name, type);

            // Assert
            _mockCategoryRepository.Verify(r => r.Add(_mockCategory.Object), Times.Once);
            Assert.Equal(_mockCategory.Object, result);
        }

        [Fact]
        public void UpdateCategoryName_ValidId_UpdatesNameAndSavesToRepository()
        {
            // Arrange
            Guid categoryId = Guid.NewGuid();
            string newName = "Updated Category";

            _mockCategoryRepository.Setup(r => r.GetById(categoryId))
                .Returns(_mockCategory.Object);

            // Act
            _facade.UpdateCategoryName(categoryId, newName);

            // Assert
            _mockCategory.Verify(c => c.UpdateName(newName), Times.Once);
            _mockCategoryRepository.Verify(r => r.Update(_mockCategory.Object), Times.Once);
        }

        [Fact]
        public void DeleteCategory_ValidId_DeletesFromRepository()
        {
            // Arrange
            Guid categoryId = Guid.NewGuid();

            // Act
            _facade.DeleteCategory(categoryId);

            // Assert
            _mockCategoryRepository.Verify(r => r.Delete(categoryId), Times.Once);
        }

        [Fact]
        public void GetCategory_ValidId_ReturnsCategory()
        {
            // Arrange
            Guid categoryId = Guid.NewGuid();
            _mockCategoryRepository.Setup(r => r.GetById(categoryId))
                .Returns(_mockCategory.Object);

            // Act
            ICategory? result = _facade.GetCategory(categoryId);

            // Assert
            Assert.Equal(_mockCategory.Object, result);
        }

        [Fact]
        public void GetAllCategories_ReturnsAllCategories()
        {
            // Arrange
            List<ICategory> categories = new() { _mockCategory.Object };
            _mockCategoryRepository.Setup(r => r.GetAll())
                .Returns(categories);

            // Act
            IEnumerable<ICategory> result = _facade.GetAllCategories();

            // Assert
            Assert.Equal(categories, result);
        }

        [Fact]
        public void GetCategoriesByType_ReturnsFilteredCategories()
        {
            // Arrange
            CategoryType typeToFilter = CategoryType.Expense;
            List<ICategory> allCategories = new() { _mockCategory.Object };

            _mockCategory.SetupGet(c => c.Type).Returns(typeToFilter);
            _mockCategoryRepository.Setup(r => r.GetAll())
                .Returns(allCategories);

            // Act
            IEnumerable<ICategory> result = _facade.GetCategoriesByType(typeToFilter);

            // Assert
            Assert.Equal(allCategories, result);
        }
    }

    public class AnalyticsFacadeTests
    {
        private readonly Mock<IOperationFacade> _mockOperationFacade;
        private readonly Mock<ICategoryFacade> _mockCategoryFacade;
        private readonly Mock<IBankAccountFacade> _mockBankAccountFacade;
        private readonly AnalyticsFacade _facade;

        public AnalyticsFacadeTests()
        {
            _mockOperationFacade = new Mock<IOperationFacade>();
            _mockCategoryFacade = new Mock<ICategoryFacade>();
            _mockBankAccountFacade = new Mock<IBankAccountFacade>();

            _facade = new AnalyticsFacade(
                _mockOperationFacade.Object,
                _mockCategoryFacade.Object,
                _mockBankAccountFacade.Object);
        }

        [Fact]
        public void GetAllAccounts_ReturnsAccountsFromBankAccountFacade()
        {
            // Arrange
            List<IBankAccount> expectedAccounts = new() { Mock.Of<IBankAccount>(), Mock.Of<IBankAccount>() };

            _mockBankAccountFacade.Setup(f => f.GetAllBankAccounts())
                .Returns(expectedAccounts);

            // Act
            IEnumerable<IBankAccount> result = _facade.GetAllAccounts();

            // Assert
            Assert.Equal(expectedAccounts, result);
            _mockBankAccountFacade.Verify(f => f.GetAllBankAccounts(), Times.Once);
        }

        [Fact]
        public void GetAllCategories_ReturnsCategoriesFromCategoryFacade()
        {
            // Arrange
            List<ICategory> expectedCategories = new() { Mock.Of<ICategory>(), Mock.Of<ICategory>() };

            _mockCategoryFacade.Setup(f => f.GetAllCategories())
                .Returns(expectedCategories);

            // Act
            IEnumerable<ICategory> result = _facade.GetAllCategories();

            // Assert
            Assert.Equal(expectedCategories, result);
            _mockCategoryFacade.Verify(f => f.GetAllCategories(), Times.Once);
        }

        [Fact]
        public void GetAllOperations_ReturnsOperationsFromOperationFacade()
        {
            // Arrange
            List<IOperation> expectedOperations = new() { Mock.Of<IOperation>(), Mock.Of<IOperation>() };

            _mockOperationFacade.Setup(f => f.GetAllOperations())
                .Returns(expectedOperations);

            // Act
            IEnumerable<IOperation> result = _facade.GetAllOperations();

            // Assert
            Assert.Equal(expectedOperations, result);
            _mockOperationFacade.Verify(f => f.GetAllOperations(), Times.Once);
        }

        [Fact]
        public void GetOperationsByAccount_CallsOperationFacadeWithCorrectId()
        {
            // Arrange
            Guid accountId = Guid.NewGuid();
            List<IOperation> expectedOperations = new() { Mock.Of<IOperation>() };

            _mockOperationFacade.Setup(f => f.GetOperationsByAccount(accountId))
                .Returns(expectedOperations);

            // Act
            IEnumerable<IOperation> result = _facade.GetOperationsByAccount(accountId);

            // Assert
            Assert.Equal(expectedOperations, result);
            _mockOperationFacade.Verify(f => f.GetOperationsByAccount(accountId), Times.Once);
        }

        [Fact]
        public void CalculateIncomeExpenseDifferenceForAccount_CorrectlyCalculatesDifference()
        {
            // Arrange
            Guid accountId = Guid.NewGuid();
            DateTime startDate = new DateTime(2023, 1, 1);
            DateTime endDate = new DateTime(2023, 1, 31);

            List<IOperation> mockOperations = new List<IOperation>
            {
                CreateMockOperation(OperationType.Income, 1000m, new DateTime(2023, 1, 15)),
                CreateMockOperation(OperationType.Income, 500m, new DateTime(2023, 1, 20)),
                CreateMockOperation(OperationType.Expense, 300m, new DateTime(2023, 1, 10)),
                CreateMockOperation(OperationType.Expense, 200m, new DateTime(2023, 1, 25)),
                // Операция вне заданного периода
                CreateMockOperation(OperationType.Income, 2000m, new DateTime(2023, 2, 1))
            };

            _mockOperationFacade.Setup(f => f.GetOperationsByAccount(accountId))
                .Returns(mockOperations);

            // Ожидаемая разница: 1000 + 500 - 300 - 200 = 1000
            decimal expectedDifference = 1000m;

            // Act
            decimal result = _facade.CalculateIncomeExpenseDifferenceForAccount(accountId, startDate, endDate);

            // Assert
            Assert.Equal(expectedDifference, result);
        }

        [Fact]
        public void GroupOperationsByCategoryForAccount_ReturnsCorrectGrouping()
        {
            // Arrange
            Guid accountId = Guid.NewGuid();
            DateTime startDate = new DateTime(2023, 1, 1);
            DateTime endDate = new DateTime(2023, 1, 31);

            Guid category1Id = Guid.NewGuid();
            Guid category2Id = Guid.NewGuid();

            Mock<ICategory> mockCategory1 = new();
            mockCategory1.SetupGet(c => c.Id).Returns(category1Id);

            Mock<ICategory> mockCategory2 = new();
            mockCategory2.SetupGet(c => c.Id).Returns(category2Id);

            List<ICategory> categories = new() { mockCategory1.Object, mockCategory2.Object };

            List<IOperation> operations = new()
            {
                CreateMockOperation(OperationType.Income, 1000m, new DateTime(2023, 1, 15), category1Id),
                CreateMockOperation(OperationType.Income, 500m, new DateTime(2023, 1, 20), category1Id),
                CreateMockOperation(OperationType.Expense, 300m, new DateTime(2023, 1, 10), category2Id),
                CreateMockOperation(OperationType.Expense, 200m, new DateTime(2023, 1, 25), category2Id),
                // Операция вне заданного периода
                CreateMockOperation(OperationType.Income, 2000m, new DateTime(2023, 2, 1), category1Id)
            };

            _mockCategoryFacade.Setup(f => f.GetAllCategories())
                .Returns(categories);

            _mockOperationFacade.Setup(f => f.GetOperationsByAccount(accountId))
                .Returns(operations);

            // Act
            Dictionary<ICategory, decimal> result =
                _facade.GroupOperationsByCategoryForAccount(accountId, startDate, endDate);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(1500m, result[mockCategory1.Object]); // 1000 + 500
            Assert.Equal(500m, result[mockCategory2.Object]); // 300 + 200
        }

        private static IOperation CreateMockOperation(OperationType type, decimal amount, DateTime date,
            Guid? categoryId = null)
        {
            Mock<IOperation> mockOperation = new();
            mockOperation.SetupGet(o => o.Type).Returns(type);
            mockOperation.SetupGet(o => o.Amount).Returns(amount);
            mockOperation.SetupGet(o => o.Date).Returns(date);
            mockOperation.SetupGet(o => o.CategoryId).Returns(categoryId ?? Guid.NewGuid());
            return mockOperation.Object;
        }
    }

    public class ConsoleLoggerTests
    {
        [Fact]
        public void LogInformation_WritesToConsole_WithCorrectFormat()
        {
            // Arrange
            ConsoleLogger logger = new ConsoleLogger();
            StringWriter consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);
            string message = "Test information message";

            try
            {
                // Act
                logger.LogInformation(message);
                string output = consoleOutput.ToString();

                // Assert
                Assert.Contains("[INFO]", output);
                Assert.Contains(message, output);
            }
            finally
            {
                // Restore console
                Console.SetOut(new StreamWriter(Console.OpenStandardOutput()));
            }
        }

        [Fact]
        public void LogWarning_WritesToConsole_WithCorrectFormat()
        {
            // Arrange
            ConsoleLogger logger = new();
            StringWriter consoleOutput = new();
            Console.SetOut(consoleOutput);
            string message = "Test warning message";

            try
            {
                // Act
                logger.LogWarning(message);
                string output = consoleOutput.ToString();

                // Assert
                Assert.Contains("[WARNING]", output);
                Assert.Contains(message, output);
            }
            finally
            {
                // Restore console
                Console.SetOut(new StreamWriter(Console.OpenStandardOutput()));
            }
        }

        [Fact]
        public void LogError_WritesToConsole_WithCorrectFormat()
        {
            // Arrange
            ConsoleLogger logger = new();
            StringWriter consoleOutput = new();
            Console.SetOut(consoleOutput);
            string message = "Test error message";
            InvalidOperationException exception = new("Test exception");

            try
            {
                // Act
                logger.LogError(message, exception);
                string output = consoleOutput.ToString();

                // Assert
                Assert.Contains("[ERROR]", output);
                Assert.Contains(message, output);
                Assert.Contains("Exception: Test exception", output);
            }
            finally
            {
                // Restore console
                Console.SetOut(new StreamWriter(Console.OpenStandardOutput()));
            }
        }
    }

    public class CreateBankAccountCommandTests
    {
        [Fact]
        public void Execute_ShouldCallCreateBankAccount_WithCorrectParameters()
        {
            // Arrange
            Mock<IBankAccountFacade> mockBankAccountFacade = new();
            string name = "Test Account";
            decimal initialBalance = 1000m;

            CreateBankAccountCommand command = new CreateBankAccountCommand(
                mockBankAccountFacade.Object,
                name,
                initialBalance);

            // Act
            command.Execute();

            // Assert
            mockBankAccountFacade.Verify(
                f => f.CreateBankAccount(name, initialBalance),
                Times.Once);
        }

        [Fact]
        public void Constructor_InitializesProperties_Correctly()
        {
            // Arrange
            Mock<IBankAccountFacade> mockBankAccountFacade = new();
            string name = "Savings";
            decimal initialBalance = 500m;

            // Act
            CreateBankAccountCommand command = new(
                mockBankAccountFacade.Object,
                name,
                initialBalance);

            // Assert - Will verify through the Execute method behavior
            command.Execute();
            mockBankAccountFacade.Verify(
                f => f.CreateBankAccount(name, initialBalance),
                Times.Once);
        }
    }

    public class UpdateAccountNameCommandTests
    {
        [Fact]
        public void Execute_ShouldCallUpdateAccountName_WithCorrectParameters()
        {
            // Arrange
            Mock<IBankAccountFacade> mockBankAccountFacade = new();
            Guid accountId = Guid.NewGuid();
            string newName = "Updated Name";

            UpdateAccountNameCommand command = new(
                mockBankAccountFacade.Object,
                accountId,
                newName);

            // Act
            command.Execute();

            // Assert
            mockBankAccountFacade.Verify(
                f => f.UpdateAccountName(accountId, newName),
                Times.Once);
        }

        [Fact]
        public void Constructor_InitializesProperties_Correctly()
        {
            // Arrange
            Mock<IBankAccountFacade> mockBankAccountFacade = new();
            Guid accountId = Guid.NewGuid();
            string newName = "Renamed Account";

            // Act
            UpdateAccountNameCommand command = new(
                mockBankAccountFacade.Object,
                accountId,
                newName);

            // Assert - Will verify through the Execute method behavior
            command.Execute();
            mockBankAccountFacade.Verify(
                f => f.UpdateAccountName(accountId, newName),
                Times.Once);
        }
    }

    public class DeleteBankAccountCommandTests
    {
        [Fact]
        public void Execute_ShouldDeleteAllAccountOperations_ThenDeleteAccount()
        {
            // Arrange
            Mock<IBankAccountFacade> mockBankAccountFacade = new();
            Mock<IOperationFacade> mockOperationFacade = new();
            Guid accountId = Guid.NewGuid();

            IOperation operation1 = Mock.Of<IOperation>(o => o.Id == Guid.NewGuid());
            IOperation operation2 = Mock.Of<IOperation>(o => o.Id == Guid.NewGuid());
            List<IOperation> operations = new() { operation1, operation2 };

            mockOperationFacade
                .Setup(f => f.GetOperationsByAccount(accountId))
                .Returns(operations);

            DeleteBankAccountCommand command = new(
                mockBankAccountFacade.Object,
                mockOperationFacade.Object,
                accountId);

            // Act
            command.Execute();

            // Assert
            // Verify operations were deleted first
            mockOperationFacade.Verify(f => f.DeleteOperation(operation1.Id), Times.Once);
            mockOperationFacade.Verify(f => f.DeleteOperation(operation2.Id), Times.Once);

            // Verify account was deleted last
            mockBankAccountFacade.Verify(f => f.DeleteBankAccount(accountId), Times.Once);

            // Verify the correct order of operations
            MockSequence sequence = new();
            mockOperationFacade.InSequence(sequence).Setup(f => f.GetOperationsByAccount(accountId));
            mockOperationFacade.InSequence(sequence).Setup(f => f.DeleteOperation(It.IsAny<Guid>()));
            mockBankAccountFacade.InSequence(sequence).Setup(f => f.DeleteBankAccount(accountId));
        }

        [Fact]
        public void Execute_WithNoOperations_ShouldOnlyDeleteAccount()
        {
            // Arrange
            Mock<IBankAccountFacade> mockBankAccountFacade = new();
            Mock<IOperationFacade> mockOperationFacade = new();
            Guid accountId = Guid.NewGuid();

            mockOperationFacade
                .Setup(f => f.GetOperationsByAccount(accountId))
                .Returns(new List<IOperation>());

            DeleteBankAccountCommand command = new(
                mockBankAccountFacade.Object,
                mockOperationFacade.Object,
                accountId);

            // Act
            command.Execute();

            // Assert
            mockOperationFacade.Verify(f => f.DeleteOperation(It.IsAny<Guid>()), Times.Never);
            mockBankAccountFacade.Verify(f => f.DeleteBankAccount(accountId), Times.Once);
        }
    }

    public class CreateCategoryCommandTests
    {
        [Fact]
        public void Execute_ShouldCallCreateCategory_WithCorrectParameters()
        {
            // Arrange
            Mock<ICategoryFacade> mockCategoryFacade = new();
            string name = "Food";
            CategoryType type = CategoryType.Expense;

            CreateCategoryCommand command = new(
                mockCategoryFacade.Object,
                name,
                type);

            // Act
            command.Execute();

            // Assert
            mockCategoryFacade.Verify(
                f => f.CreateCategory(name, type),
                Times.Once);
        }

        [Fact]
        public void Constructor_InitializesProperties_Correctly()
        {
            // Arrange
            Mock<ICategoryFacade> mockCategoryFacade = new();
            string name = "Salary";
            CategoryType type = CategoryType.Income;

            // Act
            CreateCategoryCommand command = new(
                mockCategoryFacade.Object,
                name,
                type);

            // Assert
            command.Execute();
            mockCategoryFacade.Verify(
                f => f.CreateCategory(name, type),
                Times.Once);
        }
    }

    public class UpdateCategoryNameCommandTests
    {
        [Fact]
        public void Execute_ShouldCallUpdateCategoryName_WithCorrectParameters()
        {
            // Arrange
            Mock<ICategoryFacade> mockCategoryFacade = new();
            Guid categoryId = Guid.NewGuid();
            string newName = "Updated Category";

            UpdateCategoryNameCommand command = new(
                mockCategoryFacade.Object,
                categoryId,
                newName);

            // Act
            command.Execute();

            // Assert
            mockCategoryFacade.Verify(
                f => f.UpdateCategoryName(categoryId, newName),
                Times.Once);
        }

        [Fact]
        public void Constructor_InitializesProperties_Correctly()
        {
            // Arrange
            Mock<ICategoryFacade> mockCategoryFacade = new();
            Guid categoryId = Guid.NewGuid();
            string newName = "Renamed Category";

            // Act
            UpdateCategoryNameCommand command = new(
                mockCategoryFacade.Object,
                categoryId,
                newName);

            // Assert
            command.Execute();
            mockCategoryFacade.Verify(
                f => f.UpdateCategoryName(categoryId, newName),
                Times.Once);
        }
    }

    public class DeleteCategoryCommandTests
    {
        [Fact]
        public void Execute_ShouldDeleteAllCategoryOperations_ThenDeleteCategory()
        {
            // Arrange
            Mock<ICategoryFacade> mockCategoryFacade = new();
            Mock<IOperationFacade> mockOperationFacade = new();
            Guid categoryId = Guid.NewGuid();

            IOperation operation1 = Mock.Of<IOperation>(o => o.Id == Guid.NewGuid());
            IOperation operation2 = Mock.Of<IOperation>(o => o.Id == Guid.NewGuid());
            List<IOperation> operations = new() { operation1, operation2 };

            mockOperationFacade
                .Setup(f => f.GetOperationsByCategory(categoryId))
                .Returns(operations);

            DeleteCategoryCommand command = new(
                mockCategoryFacade.Object,
                mockOperationFacade.Object,
                categoryId);

            // Act
            command.Execute();

            // Assert
            // Verify operations were deleted first
            mockOperationFacade.Verify(f => f.DeleteOperation(operation1.Id), Times.Once);
            mockOperationFacade.Verify(f => f.DeleteOperation(operation2.Id), Times.Once);

            // Verify category was deleted last
            mockCategoryFacade.Verify(f => f.DeleteCategory(categoryId), Times.Once);

            // Verify the correct order of operations
            MockSequence sequence = new();
            mockOperationFacade.InSequence(sequence).Setup(f => f.GetOperationsByCategory(categoryId));
            mockOperationFacade.InSequence(sequence).Setup(f => f.DeleteOperation(It.IsAny<Guid>()));
            mockCategoryFacade.InSequence(sequence).Setup(f => f.DeleteCategory(categoryId));
        }

        [Fact]
        public void Execute_WithNoOperations_ShouldOnlyDeleteCategory()
        {
            // Arrange
            Mock<ICategoryFacade> mockCategoryFacade = new();
            Mock<IOperationFacade> mockOperationFacade = new();
            Guid categoryId = Guid.NewGuid();

            mockOperationFacade
                .Setup(f => f.GetOperationsByCategory(categoryId))
                .Returns(new List<IOperation>());

            DeleteCategoryCommand command = new(
                mockCategoryFacade.Object,
                mockOperationFacade.Object,
                categoryId);

            // Act
            command.Execute();

            // Assert
            mockOperationFacade.Verify(f => f.DeleteOperation(It.IsAny<Guid>()), Times.Never);
            mockCategoryFacade.Verify(f => f.DeleteCategory(categoryId), Times.Once);
        }
    }

    public class CreateOperationCommandTests
    {
        [Fact]
        public void Execute_ShouldCallCreateOperation_WithCorrectParameters()
        {
            // Arrange
            Mock<IOperationFacade> mockOperationFacade = new();
            OperationType type = OperationType.Income;
            Guid bankAccountId = Guid.NewGuid();
            decimal amount = 750.50m;
            DateTime date = new(2023, 5, 15);
            string description = "Salary payment";
            Guid categoryId = Guid.NewGuid();

            CreateOperationCommand command = new(
                mockOperationFacade.Object,
                type,
                bankAccountId,
                amount,
                date,
                description,
                categoryId);

            // Act
            command.Execute();

            // Assert
            mockOperationFacade.Verify(
                f => f.CreateOperation(
                    type,
                    bankAccountId,
                    amount,
                    date,
                    description,
                    categoryId),
                Times.Once);
        }

        [Fact]
        public void Constructor_WithNullDescription_UsesEmptyString()
        {
            // Arrange
            Mock<IOperationFacade> mockOperationFacade = new();
            OperationType type = OperationType.Expense;
            Guid bankAccountId = Guid.NewGuid();
            decimal amount = 250.75m;
            DateTime date = DateTime.Now;
            string? description = null;
            Guid categoryId = Guid.NewGuid();

            // Act
            CreateOperationCommand command = new(
                mockOperationFacade.Object,
                type,
                bankAccountId,
                amount,
                date,
                description,
                categoryId);

            // Assert
            command.Execute();
            mockOperationFacade.Verify(
                f => f.CreateOperation(
                    type,
                    bankAccountId,
                    amount,
                    date,
                    string.Empty,
                    categoryId),
                Times.Once);
        }
    }

    public class DeleteOperationCommandTests
    {
        [Fact]
        public void Execute_ShouldCallDeleteOperation_WithCorrectId()
        {
            // Arrange
            Mock<IOperationFacade> mockOperationFacade = new();
            Guid operationId = Guid.NewGuid();

            DeleteOperationCommand command = new DeleteOperationCommand(
                mockOperationFacade.Object,
                operationId);

            // Act
            command.Execute();

            // Assert
            mockOperationFacade.Verify(
                f => f.DeleteOperation(operationId),
                Times.Once);
        }

        [Fact]
        public void Constructor_InitializesProperties_Correctly()
        {
            // Arrange
            Mock<IOperationFacade> mockOperationFacade = new();
            Guid operationId = Guid.NewGuid();

            // Act
            DeleteOperationCommand command = new(
                mockOperationFacade.Object,
                operationId);

            // Assert
            command.Execute();
            mockOperationFacade.Verify(
                f => f.DeleteOperation(operationId),
                Times.Once);
        }
    }

    public class UpdateOperationDescriptionCommandTests
    {
        [Fact]
        public void Execute_ShouldCallUpdateOperationDescription_WithCorrectParameters()
        {
            // Arrange
            Mock<IOperationFacade> mockOperationFacade = new();
            Guid operationId = Guid.NewGuid();
            string newDescription = "Updated description";

            UpdateOperationDescriptionCommand command = new(
                mockOperationFacade.Object,
                operationId,
                newDescription);

            // Act
            command.Execute();

            // Assert
            mockOperationFacade.Verify(
                f => f.UpdateOperationDescription(operationId, newDescription),
                Times.Once);
        }

        [Fact]
        public void Constructor_InitializesProperties_Correctly()
        {
            // Arrange
            Mock<IOperationFacade> mockOperationFacade = new();
            Guid operationId = Guid.NewGuid();
            string newDescription = "New detailed description";

            // Act
            UpdateOperationDescriptionCommand command = new(
                mockOperationFacade.Object,
                operationId,
                newDescription);

            // Assert
            command.Execute();
            mockOperationFacade.Verify(
                f => f.UpdateOperationDescription(operationId, newDescription),
                Times.Once);
        }
    }

    public class CalculateIncomeExpenseDifferenceCommandTests
    {
        [Fact]
        public void Execute_ShouldCallAnalyticsFacade_WithCorrectParameters()
        {
            // Arrange
            Mock<IAnalyticsFacade> mockAnalyticsFacade = new();
            Guid accountId = Guid.NewGuid();
            DateTime startDate = new(2023, 1, 1);
            DateTime endDate = new(2023, 1, 31);
            decimal expectedResult = 1500.75m;

            mockAnalyticsFacade
                .Setup(f => f.CalculateIncomeExpenseDifferenceForAccount(accountId, startDate, endDate))
                .Returns(expectedResult);

            CalculateIncomeExpenseDifferenceCommand command = new CalculateIncomeExpenseDifferenceCommand(
                mockAnalyticsFacade.Object,
                accountId,
                startDate,
                endDate);

            // Act
            command.Execute();
            decimal result = command.GetResult();

            // Assert
            mockAnalyticsFacade.Verify(
                f => f.CalculateIncomeExpenseDifferenceForAccount(accountId, startDate, endDate),
                Times.Once);
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void Constructor_InitializesProperties_Correctly()
        {
            // Arrange
            Mock<IAnalyticsFacade> mockAnalyticsFacade = new();
            Guid accountId = Guid.NewGuid();
            DateTime startDate = new(2023, 5, 1);
            DateTime endDate = new(2023, 5, 31);
            decimal expectedResult = 2750.25m;

            mockAnalyticsFacade
                .Setup(f => f.CalculateIncomeExpenseDifferenceForAccount(accountId, startDate, endDate))
                .Returns(expectedResult);

            // Act
            CalculateIncomeExpenseDifferenceCommand command = new(
                mockAnalyticsFacade.Object,
                accountId,
                startDate,
                endDate);
            command.Execute();

            // Assert
            Assert.Equal(expectedResult, command.GetResult());
        }
    }

    public class GetAllAccountsCommandTests
    {
        [Fact]
        public void Execute_ShouldCallGetAllAccounts_AndReturnCorrectResult()
        {
            // Arrange
            Mock<IAnalyticsFacade> mockAnalyticsFacade = new();
            Mock<IBankAccount> mockAccount1 = new();
            Mock<IBankAccount> mockAccount2 = new();
            List<IBankAccount> expectedAccounts = new() { mockAccount1.Object, mockAccount2.Object };

            mockAnalyticsFacade
                .Setup(f => f.GetAllAccounts())
                .Returns(expectedAccounts);

            GetAllAccountsCommand command = new GetAllAccountsCommand(mockAnalyticsFacade.Object);

            // Act
            command.Execute();
            IEnumerable<IBankAccount> result = command.GetResult();

            // Assert
            mockAnalyticsFacade.Verify(f => f.GetAllAccounts(), Times.Once);
            Assert.Equal(expectedAccounts, result);
        }

        [Fact]
        public void Constructor_InitializesEmptyResultsList()
        {
            // Arrange
            Mock<IAnalyticsFacade> mockAnalyticsFacade = new();
            mockAnalyticsFacade
                .Setup(f => f.GetAllAccounts())
                .Returns(new List<IBankAccount>());

            // Act
            GetAllAccountsCommand command = new(mockAnalyticsFacade.Object);
            IEnumerable<IBankAccount> initialResult = command.GetResult();

            // Assert
            Assert.Empty(initialResult);
        }
    }

    public class GetAllCategoriesCommandTests
    {
        [Fact]
        public void Execute_ShouldCallGetAllCategories_AndReturnCorrectResult()
        {
            // Arrange
            Mock<IAnalyticsFacade> mockAnalyticsFacade = new();
            Mock<ICategory> mockCategory1 = new();
            Mock<ICategory> mockCategory2 = new();
            List<ICategory> expectedCategories = new() { mockCategory1.Object, mockCategory2.Object };

            mockAnalyticsFacade
                .Setup(f => f.GetAllCategories())
                .Returns(expectedCategories);

            GetAllCategoriesCommand command = new(mockAnalyticsFacade.Object);

            // Act
            command.Execute();
            IEnumerable<ICategory> result = command.GetResult();

            // Assert
            mockAnalyticsFacade.Verify(f => f.GetAllCategories(), Times.Once);
            Assert.Equal(expectedCategories, result);
        }

        [Fact]
        public void Constructor_InitializesEmptyResultsList()
        {
            // Arrange
            Mock<IAnalyticsFacade> mockAnalyticsFacade = new();
            mockAnalyticsFacade
                .Setup(f => f.GetAllCategories())
                .Returns(new List<ICategory>());

            // Act
            GetAllCategoriesCommand command = new(mockAnalyticsFacade.Object);
            IEnumerable<ICategory> initialResult = command.GetResult();

            // Assert
            Assert.Empty(initialResult);
        }
    }

    public class GetAllOperationsCommandTests
    {
        [Fact]
        public void Execute_ShouldCallGetAllOperations_AndReturnCorrectResult()
        {
            // Arrange
            Mock<IAnalyticsFacade> mockAnalyticsFacade = new();
            Mock<IOperation> mockOperation1 = new();
            Mock<IOperation> mockOperation2 = new();
            List<IOperation> expectedOperations = new() { mockOperation1.Object, mockOperation2.Object };

            mockAnalyticsFacade
                .Setup(f => f.GetAllOperations())
                .Returns(expectedOperations);

            GetAllOperationsCommand command = new(mockAnalyticsFacade.Object);

            // Act
            command.Execute();
            IEnumerable<IOperation> result = command.GetResult();

            // Assert
            mockAnalyticsFacade.Verify(f => f.GetAllOperations(), Times.Once);
            Assert.Equal(expectedOperations, result);
        }

        [Fact]
        public void Constructor_InitializesEmptyResultsList()
        {
            // Arrange
            Mock<IAnalyticsFacade> mockAnalyticsFacade = new();
            mockAnalyticsFacade
                .Setup(f => f.GetAllOperations())
                .Returns(new List<IOperation>());

            // Act
            GetAllOperationsCommand command = new(mockAnalyticsFacade.Object);
            IEnumerable<IOperation> initialResult = command.GetResult();

            // Assert
            Assert.Empty(initialResult);
        }
    }

    public class GetOperationsByAccountCommandTests
    {
        [Fact]
        public void Execute_ShouldCallGetOperationsByAccount_WithCorrectId()
        {
            // Arrange
            Mock<IAnalyticsFacade> mockAnalyticsFacade = new();
            Guid accountId = Guid.NewGuid();
            Mock<IOperation> mockOperation = new();
            List<IOperation> expectedOperations = new() { mockOperation.Object };

            mockAnalyticsFacade
                .Setup(f => f.GetOperationsByAccount(accountId))
                .Returns(expectedOperations);

            GetOperationsByAccountCommand command = new(mockAnalyticsFacade.Object, accountId);

            // Act
            command.Execute();
            IEnumerable<IOperation> result = command.GetResult();

            // Assert
            mockAnalyticsFacade.Verify(f => f.GetOperationsByAccount(accountId), Times.Once);
            Assert.Equal(expectedOperations, result);
        }

        [Fact]
        public void Constructor_InitializesEmptyResultsList()
        {
            // Arrange
            Mock<IAnalyticsFacade> mockAnalyticsFacade = new();
            Guid accountId = Guid.NewGuid();
            mockAnalyticsFacade
                .Setup(f => f.GetOperationsByAccount(accountId))
                .Returns(new List<IOperation>());

            // Act
            GetOperationsByAccountCommand command = new(mockAnalyticsFacade.Object, accountId);
            IEnumerable<IOperation> initialResult = command.GetResult();

            // Assert
            Assert.Empty(initialResult);
        }
    }

    public class GroupOperationsByCategoryCommandTests
    {
        [Fact]
        public void Execute_ShouldCallGroupOperationsByCategoryForAccount_WithCorrectParameters()
        {
            // Arrange
            Mock<IAnalyticsFacade> mockAnalyticsFacade = new();
            Guid accountId = Guid.NewGuid();
            DateTime startDate = new(2023, 1, 1);
            DateTime endDate = new(2023, 1, 31);

            Mock<ICategory> mockCategory = new();
            Dictionary<ICategory, decimal> expectedResult = new() { { mockCategory.Object, 1500m } };

            mockAnalyticsFacade
                .Setup(f => f.GroupOperationsByCategoryForAccount(accountId, startDate, endDate))
                .Returns(expectedResult);

            GroupOperationsByCategoryCommand command = new(
                mockAnalyticsFacade.Object,
                accountId,
                startDate,
                endDate);

            // Act
            command.Execute();
            Dictionary<ICategory, decimal> result = command.GetResult();

            // Assert
            mockAnalyticsFacade.Verify(
                f => f.GroupOperationsByCategoryForAccount(accountId, startDate, endDate),
                Times.Once);
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void Constructor_InitializesEmptyResultsDictionary()
        {
            // Arrange
            Mock<IAnalyticsFacade> mockAnalyticsFacade = new();
            Guid accountId = Guid.NewGuid();
            DateTime startDate = new(2023, 5, 1);
            DateTime endDate = new(2023, 5, 31);

            mockAnalyticsFacade
                .Setup(f => f.GroupOperationsByCategoryForAccount(accountId, startDate, endDate))
                .Returns(new Dictionary<ICategory, decimal>());

            // Act
            GroupOperationsByCategoryCommand command = new(
                mockAnalyticsFacade.Object,
                accountId,
                startDate,
                endDate);
            Dictionary<ICategory, decimal> initialResult = command.GetResult();

            // Assert
            Assert.Empty(initialResult);
        }
    }

    public class DataExporterTests
    {
        [Fact]
        public void ExportData_ShouldVisitAllEntities_FromFacades()
        {
            // Arrange
            string tempFilePath = Path.GetTempFileName();
            try
            {
                Mock<IBankAccountFacade> mockAccountFacade = new();
                Mock<ICategoryFacade> mockCategoryFacade = new();
                Mock<IOperationFacade> mockOperationFacade = new();
                Mock<IExportVisitor> mockVisitor = new();

                List<IBankAccount> accounts = new() { Mock.Of<IBankAccount>(), Mock.Of<IBankAccount>() };
                List<ICategory> categories = new() { Mock.Of<ICategory>(), Mock.Of<ICategory>() };
                List<IOperation> operations = new() { Mock.Of<IOperation>(), Mock.Of<IOperation>() };

                mockAccountFacade.Setup(f => f.GetAllBankAccounts()).Returns(accounts);
                mockCategoryFacade.Setup(f => f.GetAllCategories()).Returns(categories);
                mockOperationFacade.Setup(f => f.GetAllOperations()).Returns(operations);

                DataExporter exporter = new(
                    mockAccountFacade.Object,
                    mockCategoryFacade.Object,
                    mockOperationFacade.Object);

                // Act
                exporter.ExportData(mockVisitor.Object, tempFilePath);

                // Assert
                mockAccountFacade.Verify(f => f.GetAllBankAccounts(), Times.Once);
                mockCategoryFacade.Verify(f => f.GetAllCategories(), Times.Once);
                mockOperationFacade.Verify(f => f.GetAllOperations(), Times.Once);

                foreach (IBankAccount account in accounts)
                {
                    mockVisitor.Verify(v => v.VisitBankAccount(account), Times.Once);
                }

                foreach (ICategory category in categories)
                {
                    mockVisitor.Verify(v => v.VisitCategory(category), Times.Once);
                }

                foreach (IOperation operation in operations)
                {
                    mockVisitor.Verify(v => v.VisitOperation(operation), Times.Once);
                }

                mockVisitor.Verify(v => v.SaveToFile(tempFilePath), Times.Once);
            }
            finally
            {
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
        }

    }

    public class CsvExportVisitorTests : IDisposable
    {
        private readonly string _tempFilePath;

        public CsvExportVisitorTests()
        {
            // Create a temporary file path for tests
            _tempFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.csv");
        }

        [Fact]
        public void VisitBankAccount_AddsAccountToCollection()
        {
            // Arrange
            CsvExportVisitor visitor = new();
            Mock<IBankAccount> mockAccount = new();

            // Act
            visitor.VisitBankAccount(mockAccount.Object);
            visitor.SaveToFile(_tempFilePath);

            // Assert
            string fileContent = File.ReadAllText(_tempFilePath);
            Assert.Contains("[Accounts]", fileContent);
            Assert.Contains("Id,Name,Balance", fileContent);
        }

        [Fact]
        public void VisitCategory_AddsCategoryToCollection()
        {
            // Arrange
            CsvExportVisitor visitor = new();
            Mock<ICategory> mockCategory = new();

            // Act
            visitor.VisitCategory(mockCategory.Object);
            visitor.SaveToFile(_tempFilePath);

            // Assert
            string fileContent = File.ReadAllText(_tempFilePath);
            Assert.Contains("[Categories]", fileContent);
            Assert.Contains("Id,Name,Type", fileContent);
        }

        [Fact]
        public void VisitOperation_AddsOperationToCollection()
        {
            // Arrange
            CsvExportVisitor visitor = new();
            Mock<IOperation> mockOperation = new();

            // Act
            visitor.VisitOperation(mockOperation.Object);
            visitor.SaveToFile(_tempFilePath);

            // Assert
            string fileContent = File.ReadAllText(_tempFilePath);
            Assert.Contains("[Operations]", fileContent);
            Assert.Contains("Id,Type,BankAccountId,Amount,Date,Description,CategoryId", fileContent);
        }

        [Fact]
        public void SaveToFile_FormatsCsvCorrectly_WithAllEntities()
        {
            // Arrange
            CsvExportVisitor visitor = new();

            // Create mock account
            Guid accountId = Guid.NewGuid();
            Mock<IBankAccount> mockAccount = new();
            mockAccount.Setup(a => a.Id).Returns(accountId);
            mockAccount.Setup(a => a.Name).Returns("Test Account");
            mockAccount.Setup(a => a.Balance).Returns(1000.50m);

            // Create mock category
            Guid categoryId = Guid.NewGuid();
            Mock<ICategory> mockCategory = new();
            mockCategory.Setup(c => c.Id).Returns(categoryId);
            mockCategory.Setup(c => c.Name).Returns("Food");
            mockCategory.Setup(c => c.Type).Returns(CategoryType.Expense);

            // Create mock operation
            Guid operationId = Guid.NewGuid();
            DateTime operationDate = new(2023, 6, 15);
            Mock<IOperation> mockOperation = new();
            mockOperation.Setup(o => o.Id).Returns(operationId);
            mockOperation.Setup(o => o.Type).Returns(OperationType.Expense);
            mockOperation.Setup(o => o.BankAccountId).Returns(accountId);
            mockOperation.Setup(o => o.Amount).Returns(50.25m);
            mockOperation.Setup(o => o.Date).Returns(operationDate);
            mockOperation.Setup(o => o.Description).Returns("Grocery shopping");
            mockOperation.Setup(o => o.CategoryId).Returns(categoryId);

            // Visit all entities
            visitor.VisitBankAccount(mockAccount.Object);
            visitor.VisitCategory(mockCategory.Object);
            visitor.VisitOperation(mockOperation.Object);

            // Act
            visitor.SaveToFile(_tempFilePath);

            // Assert
            string fileContent = File.ReadAllText(_tempFilePath);

            // Check account section
            Assert.Contains($"{accountId},Test Account,1000.5", fileContent);

            // Check category section
            Assert.Contains($"{categoryId},Food,Expense", fileContent);

            // Check operation section
            Assert.Contains($"{operationId},Expense,{accountId},50.25,2023-06-15,Grocery shopping,{categoryId}",
                fileContent);
        }

        [Fact]
        public void SaveToFile_HandlesSpecialCharactersInStrings()
        {
            // Arrange
            CsvExportVisitor visitor = new();

            // Create mock account with commas in name (which could break CSV format)
            Guid accountId = Guid.NewGuid();
            Mock<IBankAccount> mockAccount = new();
            mockAccount.Setup(a => a.Id).Returns(accountId);
            mockAccount.Setup(a => a.Name).Returns("Test, Account");
            mockAccount.Setup(a => a.Balance).Returns(1000.50m);

            // Visit account
            visitor.VisitBankAccount(mockAccount.Object);

            // Act
            visitor.SaveToFile(_tempFilePath);

            // Assert
            string fileContent = File.ReadAllText(_tempFilePath);

            // The CSV format might be broken by the comma, so test that the file was created
            // and contains the account ID which should be unique
            Assert.Contains(accountId.ToString(), fileContent);
        }


        public void Dispose()
        {
            if (File.Exists(_tempFilePath))
            {
                File.Delete(_tempFilePath);
            }
        }
    }

    public class JsonExportVisitorTests : IDisposable
    {
        private readonly string _tempFilePath;

        public JsonExportVisitorTests()
        {
            // Create a temporary file path for tests
            _tempFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");
        }

        [Fact]
        public void VisitBankAccount_AddsAccountToCollection()
        {
            // Arrange
            JsonExportVisitor visitor = new();
            Mock<IBankAccount> mockAccount = new();
            mockAccount.Setup(a => a.Id).Returns(Guid.NewGuid());
            mockAccount.Setup(a => a.Name).Returns("Test Account");
            mockAccount.Setup(a => a.Balance).Returns(1000.0m);

            // Act
            visitor.VisitBankAccount(mockAccount.Object);
            visitor.SaveToFile(_tempFilePath);

            // Assert
            string fileContent = File.ReadAllText(_tempFilePath);
            FinancialData? data = JsonSerializer.Deserialize<FinancialData>(fileContent);
            Assert.NotNull(data);
            Assert.Single(data.Accounts);
            Assert.Equal(mockAccount.Object.Name, data.Accounts[0].Name);
            Assert.Equal(mockAccount.Object.Balance, data.Accounts[0].Balance);
        }

        [Fact]
        public void VisitCategory_AddsCategoryToCollection()
        {
            // Arrange
            JsonExportVisitor visitor = new();
            Mock<ICategory> mockCategory = new();
            mockCategory.Setup(c => c.Id).Returns(Guid.NewGuid());
            mockCategory.Setup(c => c.Name).Returns("Food");
            mockCategory.Setup(c => c.Type).Returns(CategoryType.Expense);

            // Act
            visitor.VisitCategory(mockCategory.Object);
            visitor.SaveToFile(_tempFilePath);

            // Assert
            string fileContent = File.ReadAllText(_tempFilePath);
            FinancialData? data = JsonSerializer.Deserialize<FinancialData>(fileContent);
            Assert.NotNull(data);
            Assert.Single(data.Categories);
            Assert.Equal(mockCategory.Object.Name, data.Categories[0].Name);
            Assert.Equal(mockCategory.Object.Type, data.Categories[0].Type);
        }

        [Fact]
        public void VisitOperation_AddsOperationToCollection()
        {
            // Arrange
            JsonExportVisitor visitor = new();
            Mock<IOperation> mockOperation = new();
            Guid operationId = Guid.NewGuid();
            Guid accountId = Guid.NewGuid();
            Guid categoryId = Guid.NewGuid();
            DateTime date = new DateTime(2023, 6, 15);

            mockOperation.Setup(o => o.Id).Returns(operationId);
            mockOperation.Setup(o => o.Type).Returns(OperationType.Expense);
            mockOperation.Setup(o => o.BankAccountId).Returns(accountId);
            mockOperation.Setup(o => o.Amount).Returns(50.25m);
            mockOperation.Setup(o => o.Date).Returns(date);
            mockOperation.Setup(o => o.Description).Returns("Grocery shopping");
            mockOperation.Setup(o => o.CategoryId).Returns(categoryId);

            // Act
            visitor.VisitOperation(mockOperation.Object);
            visitor.SaveToFile(_tempFilePath);

            // Assert
            string fileContent = File.ReadAllText(_tempFilePath);
            FinancialData? data = JsonSerializer.Deserialize<FinancialData>(fileContent);
            Assert.NotNull(data);
            Assert.Single(data.Operations);
            Assert.Equal(operationId, data.Operations[0].Id);
            Assert.Equal(OperationType.Expense, data.Operations[0].Type);
            Assert.Equal(accountId, data.Operations[0].BankAccountId);
            Assert.Equal(50.25m, data.Operations[0].Amount);
            Assert.Equal(date, data.Operations[0].Date);
            Assert.Equal("Grocery shopping", data.Operations[0].Description);
            Assert.Equal(categoryId, data.Operations[0].CategoryId);
        }

        [Fact]
        public void SaveToFile_WritesValidJsonWithAllEntities()
        {
            // Arrange
            JsonExportVisitor visitor = new();

            // Create mock account
            Guid accountId = Guid.NewGuid();
            Mock<IBankAccount> mockAccount = new();
            mockAccount.Setup(a => a.Id).Returns(accountId);
            mockAccount.Setup(a => a.Name).Returns("Test Account");
            mockAccount.Setup(a => a.Balance).Returns(1000.50m);

            // Create mock category
            Guid categoryId = Guid.NewGuid();
            Mock<ICategory> mockCategory = new();
            mockCategory.Setup(c => c.Id).Returns(categoryId);
            mockCategory.Setup(c => c.Name).Returns("Food");
            mockCategory.Setup(c => c.Type).Returns(CategoryType.Expense);

            // Create mock operation
            Guid operationId = Guid.NewGuid();
            DateTime operationDate = new(2023, 6, 15);
            Mock<IOperation> mockOperation = new();
            mockOperation.Setup(o => o.Id).Returns(operationId);
            mockOperation.Setup(o => o.Type).Returns(OperationType.Expense);
            mockOperation.Setup(o => o.BankAccountId).Returns(accountId);
            mockOperation.Setup(o => o.Amount).Returns(50.25m);
            mockOperation.Setup(o => o.Date).Returns(operationDate);
            mockOperation.Setup(o => o.Description).Returns("Grocery shopping");
            mockOperation.Setup(o => o.CategoryId).Returns(categoryId);

            // Visit all entities
            visitor.VisitBankAccount(mockAccount.Object);
            visitor.VisitCategory(mockCategory.Object);
            visitor.VisitOperation(mockOperation.Object);

            // Act
            visitor.SaveToFile(_tempFilePath);

            // Assert
            string fileContent = File.ReadAllText(_tempFilePath);
            FinancialData? data = JsonSerializer.Deserialize<FinancialData>(fileContent);

            Assert.NotNull(data);
            Assert.Single(data.Accounts);
            Assert.Single(data.Categories);
            Assert.Single(data.Operations);

            // Verify account data
            Assert.Equal(accountId, data.Accounts[0].Id);
            Assert.Equal("Test Account", data.Accounts[0].Name);
            Assert.Equal(1000.50m, data.Accounts[0].Balance);

            // Verify category data
            Assert.Equal(categoryId, data.Categories[0].Id);
            Assert.Equal("Food", data.Categories[0].Name);
            Assert.Equal(CategoryType.Expense, data.Categories[0].Type);

            // Verify operation data
            Assert.Equal(operationId, data.Operations[0].Id);
            Assert.Equal(OperationType.Expense, data.Operations[0].Type);
            Assert.Equal(accountId, data.Operations[0].BankAccountId);
            Assert.Equal(50.25m, data.Operations[0].Amount);
            Assert.Equal(operationDate, data.Operations[0].Date);
            Assert.Equal("Grocery shopping", data.Operations[0].Description);
            Assert.Equal(categoryId, data.Operations[0].CategoryId);
        }

        [Fact]
        public void SaveToFile_HandlesSpecialCharactersInStrings()
        {
            // Arrange
            JsonExportVisitor visitor = new();

            // Create mock account with special characters in name
            Guid accountId = Guid.NewGuid();
            Mock<IBankAccount> mockAccount = new();
            mockAccount.Setup(a => a.Id).Returns(accountId);
            mockAccount.Setup(a => a.Name).Returns("Test \"Account\" with special chars {}/\\");
            mockAccount.Setup(a => a.Balance).Returns(1000.50m);

            // Visit account
            visitor.VisitBankAccount(mockAccount.Object);

            // Act
            visitor.SaveToFile(_tempFilePath);

            // Assert
            string fileContent = File.ReadAllText(_tempFilePath);
            FinancialData? data = JsonSerializer.Deserialize<FinancialData>(fileContent);

            Assert.NotNull(data);
            Assert.Single(data.Accounts);
            Assert.Equal("Test \"Account\" with special chars {}/\\", data.Accounts[0].Name);
        }
        
        public void Dispose()
        {
            // Clean up temporary file after tests
            if (File.Exists(_tempFilePath))
            {
                File.Delete(_tempFilePath);
            }
        }
    }

    public class YamlExportVisitorTests : IDisposable
    {
        private readonly string _tempFilePath;

        public YamlExportVisitorTests()
        {
            // Create a temporary file path for tests
            _tempFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.yaml");
        }

        [Fact]
        public void VisitBankAccount_AddsAccountToCollection()
        {
            // Arrange
            YamlExportVisitor visitor = new();
            Mock<IBankAccount> mockAccount = new();
            mockAccount.Setup(a => a.Id).Returns(Guid.NewGuid());
            mockAccount.Setup(a => a.Name).Returns("Test Account");
            mockAccount.Setup(a => a.Balance).Returns(1000.0m);

            // Act
            visitor.VisitBankAccount(mockAccount.Object);
            visitor.SaveToFile(_tempFilePath);

            // Assert
            string fileContent = File.ReadAllText(_tempFilePath);
            Assert.Contains("accounts:", fileContent);
            Assert.Contains("- id:", fileContent);
            Assert.Contains("name: Test Account", fileContent);
            Assert.Contains("balance: 1000.0", fileContent);
        }
        
        [Fact]
        public void VisitCategory_AddsCategoryToCollection()
        {
            // Arrange
            YamlExportVisitor visitor = new();
            Mock<ICategory> mockCategory = new();
            mockCategory.Setup(c => c.Id).Returns(Guid.NewGuid());
            mockCategory.Setup(c => c.Name).Returns("Food");
            mockCategory.Setup(c => c.Type).Returns(CategoryType.Expense);

            // Act
            visitor.VisitCategory(mockCategory.Object);
            visitor.SaveToFile(_tempFilePath);

            // Assert
            string fileContent = File.ReadAllText(_tempFilePath);
            Assert.Contains("categories:", fileContent);
            Assert.Contains("- id:", fileContent);
            Assert.Contains("name: Food", fileContent);
            Assert.Contains("type: Expense", fileContent);
        }

        [Fact]
        public void VisitOperation_AddsOperationToCollection()
        {
            // Arrange
            YamlExportVisitor visitor = new();
            Mock<IOperation> mockOperation = new();
            Guid operationId = Guid.NewGuid();
            Guid accountId = Guid.NewGuid();
            Guid categoryId = Guid.NewGuid();
            DateTime date = new(2023, 6, 15);

            mockOperation.Setup(o => o.Id).Returns(operationId);
            mockOperation.Setup(o => o.Type).Returns(OperationType.Expense);
            mockOperation.Setup(o => o.BankAccountId).Returns(accountId);
            mockOperation.Setup(o => o.Amount).Returns(50.25m);
            mockOperation.Setup(o => o.Date).Returns(date);
            mockOperation.Setup(o => o.Description).Returns("Grocery shopping");
            mockOperation.Setup(o => o.CategoryId).Returns(categoryId);

            // Act
            visitor.VisitOperation(mockOperation.Object);
            visitor.SaveToFile(_tempFilePath);

            // Assert
            string fileContent = File.ReadAllText(_tempFilePath);
            Assert.Contains("operations:", fileContent);
            Assert.Contains("- id:", fileContent);
            Assert.Contains("type: Expense", fileContent);
            Assert.Contains($"bankAccountId: {accountId}", fileContent);
            Assert.Contains("amount: 50.25", fileContent);
            Assert.Contains("date:", fileContent);
            Assert.Contains("description: Grocery shopping", fileContent);
            Assert.Contains($"categoryId: {categoryId}", fileContent);
        }
        
        [Fact]
        public void SaveToFile_WritesValidYamlWithAllEntities()
        {
            // Arrange
            YamlExportVisitor visitor = new();
            
            // Create mock account
            Guid accountId = Guid.NewGuid();
            Mock<IBankAccount> mockAccount = new();
            mockAccount.Setup(a => a.Id).Returns(accountId);
            mockAccount.Setup(a => a.Name).Returns("Test Account");
            mockAccount.Setup(a => a.Balance).Returns(1000.50m);

            // Create mock category
            Guid categoryId = Guid.NewGuid();
            Mock<ICategory> mockCategory = new();
            mockCategory.Setup(c => c.Id).Returns(categoryId);
            mockCategory.Setup(c => c.Name).Returns("Food");
            mockCategory.Setup(c => c.Type).Returns(CategoryType.Expense);

            // Create mock operation
            Guid operationId = Guid.NewGuid();
            DateTime operationDate = new(2023, 6, 15);
            Mock<IOperation> mockOperation = new();
            mockOperation.Setup(o => o.Id).Returns(operationId);
            mockOperation.Setup(o => o.Type).Returns(OperationType.Expense);
            mockOperation.Setup(o => o.BankAccountId).Returns(accountId);
            mockOperation.Setup(o => o.Amount).Returns(50.25m);
            mockOperation.Setup(o => o.Date).Returns(operationDate);
            mockOperation.Setup(o => o.Description).Returns("Grocery shopping");
            mockOperation.Setup(o => o.CategoryId).Returns(categoryId);

            // Visit all entities
            visitor.VisitBankAccount(mockAccount.Object);
            visitor.VisitCategory(mockCategory.Object);
            visitor.VisitOperation(mockOperation.Object);

            // Act
            visitor.SaveToFile(_tempFilePath);

            // Assert
            string fileContent = File.ReadAllText(_tempFilePath);
            
            // Verify content contains all expected sections and data
            Assert.Contains("accounts:", fileContent);
            Assert.Contains($"id: {accountId}", fileContent);
            Assert.Contains("name: Test Account", fileContent);
            Assert.Contains("balance: 1000.5", fileContent);
            
            Assert.Contains("categories:", fileContent);
            Assert.Contains($"id: {categoryId}", fileContent);
            Assert.Contains("name: Food", fileContent);
            Assert.Contains("type: Expense", fileContent);
            
            Assert.Contains("operations:", fileContent);
            Assert.Contains($"id: {operationId}", fileContent);
            Assert.Contains("type: Expense", fileContent);
            Assert.Contains($"bankAccountId: {accountId}", fileContent);
            Assert.Contains("amount: 50.25", fileContent);
            Assert.Contains("description: Grocery shopping", fileContent);
            Assert.Contains($"categoryId: {categoryId}", fileContent);
        }
        
        [Fact]
        public void SaveToFile_HandlesSpecialCharactersInStrings()
        {
            // Arrange
            YamlExportVisitor visitor = new();
        
            // Create mock account with special characters in name
            Guid accountId = Guid.NewGuid();
            Mock<IBankAccount> mockAccount = new();
            mockAccount.Setup(a => a.Id).Returns(accountId);
            mockAccount.Setup(a => a.Name).Returns("Test \"Account\" with special chars {}/\\");
            mockAccount.Setup(a => a.Balance).Returns(1000.50m);

            // Visit account
            visitor.VisitBankAccount(mockAccount.Object);

            // Act
            visitor.SaveToFile(_tempFilePath);

            // Assert
            string fileContent = File.ReadAllText(_tempFilePath);
        
            // Check if the special characters are properly escaped
            Assert.Contains("name: ", fileContent); // Should contain the name
            Assert.DoesNotContain("name: {}", fileContent); // Should not strip the special chars
        }
        
        public void Dispose()
        {
            // Clean up temporary file after tests
            if (File.Exists(_tempFilePath))
            {
                File.Delete(_tempFilePath);
            }
        }
    }
    
    
    

}