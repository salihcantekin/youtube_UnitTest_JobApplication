using JobApplicationLibrary.Models;
using NUnit.Framework;
using Moq;
using JobApplicationLibrary.Services;
using System;
using FluentAssertions;

namespace JobApplicationLibrary.UnitTest
{
    public class ApplicationEvaluateUnitTest
    {
        // UnitOfWork_Condition_ExpectedResult
        // UnitOfWork_ExpectedResult_Condition

        [Test]
        public void Application_WithUnderAge_TransferredToAutoRejected()
        {
            // Arrange
            var evaluator = new ApplicationEvaluator(null);
            var form = new JobApplication()
            {
                Applicant = new Applicant()
                {
                    Age = 17
                }
            };

            // Action
            var appResult = evaluator.Evaluate(form);

            // Assert
            //Assert.AreEqual(ApplicationResult.AutoRejected, appResult);
            appResult.Should().Be(ApplicationResult.AutoRejected);
        }


        [Test]
        public void Application_WithNoTechStack_TransferredToAutoRejected()
        {
            // Arrange
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.DefaultValue = DefaultValue.Mock;
            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("TURKEY");
            mockValidator.Setup(i => i.IsValid(It.IsAny<string>())).Returns(true);

            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant() { Age = 19 },
                TechStackList = new System.Collections.Generic.List<string>() { "" }
            };

            // Action
            var appResult = evaluator.Evaluate(form);

            // Assert
            //Assert.AreEqual(ApplicationResult.AutoRejected, appResult);
            appResult.Should().Be(ApplicationResult.AutoRejected);
        }

        [Test]
        public void Application_WithTechStackOver75P_TransferredToAutoAccepted()
        {
            // Arrange
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.DefaultValue = DefaultValue.Mock;
            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("TURKEY");
            mockValidator.Setup(i => i.IsValid(It.IsAny<string>())).Returns(true);

            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant() { Age = 19 },
                TechStackList = new System.Collections.Generic.List<string>()
                {
                    "C#", "RabbitMQ", "Microservice", "Visual Studio"
                },
                YearsOfExperience = 16
            };

            // Action
            var appResult = evaluator.Evaluate(form);

            // Assert
            //Assert.AreEqual(ApplicationResult.AutoAccepted, appResult);
            appResult.Should().Be(ApplicationResult.AutoAccepted);
        }

        [Test]
        public void Application_WithInValidIdentityNumber_TransferredToHR()
        {
            // Arrange
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.DefaultValue = DefaultValue.Mock;
            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("TURKEY");

            mockValidator.Setup(i => i.IsValid(It.IsAny<string>())).Returns(false);
            //mockValidator.Setup(i => i.CheckConnectionToRemoteServer()).Returns(false);

            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant() { Age = 19 }
            };

            // Action
            var appResult = evaluator.Evaluate(form);

            // Assert
            //Assert.AreEqual(ApplicationResult.TransferredToHR, appResult);
            appResult.Should().Be(ApplicationResult.TransferredToHR);
        }

        [Test]
        public void Application_WithOfficeLocation_TransferredToCTO()
        {
            // Arrange
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("SPAIN");

            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant() { Age = 19 }
            };

            // Action
            var appResult = evaluator.Evaluate(form);

            // Assert
            //Assert.AreEqual(ApplicationResult.TransferredToCTO, appResult); 
            appResult.Should().Be(ApplicationResult.TransferredToCTO);
        }

        [Test]
        public void Application_WithOver50_ValidationModeToDetailed()
        {
            // Arrange
            var mockValidator = new Mock<IIdentityValidator>();

            mockValidator.SetupAllProperties();

            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("SPAIN");

            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant() { Age = 51 }
            };

            // Action
            var appResult = evaluator.Evaluate(form);

            // Assert
            mockValidator.Object.ValidationMode.Should().Be(ValidationMode.Detailed);
        }

        [Test]
        public void Application_WithNullApplicatant_ThrowsArgumentNullException()
        {
            // Arrange
            var mockValidator = new Mock<IIdentityValidator>();

            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication();

            // Action
            Action appResultAction = () => evaluator.Evaluate(form).Should();

            // Assert
            appResultAction.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void Application_WithDefaultValue_IsValidCalled()
        {
            // Arrange
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.DefaultValue = DefaultValue.Mock;

            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("TURKEY");

            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant()
                {
                    Age = 19,
                    IdentityNumber = "1234"
                },

            };

            // Action
            var appResult = evaluator.Evaluate(form);

            // Assert

            mockValidator.Verify(i => i.IsValid(It.IsAny<string>()));
        }

        [Test]
        public void Application_WithYoungAge_IsValidNeverCalled()
        {
            // Arrange
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.DefaultValue = DefaultValue.Mock;

            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("TURKEY");

            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant()
                {
                    Age = 15
                }
            };

            // Action
            var appResult = evaluator.Evaluate(form);

            // Assert

            mockValidator.Verify(i => i.IsValid(It.IsAny<string>()));
        }
    }
}