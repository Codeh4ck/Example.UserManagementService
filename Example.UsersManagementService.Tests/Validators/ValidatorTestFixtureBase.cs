using System;
using System.Linq;
using NUnit.Framework;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;
using ServiceStack.FluentValidation;
using ServiceStack.FluentValidation.Results;

namespace Example.UsersManagementService.Tests.Validators
{
    public abstract class ValidatorTestFixtureBase<TValidator, TInput> where TValidator : IValidator<TInput>
    {
        protected TValidator Validator { get; set; }
        protected TInput Input { get; set; }

        protected static TestCaseData Case(string testName, Action<TInput> alterationAction)
        {
            return new TestCaseData(alterationAction).SetName(testName);
        }

        protected static TestCaseData Case<TProperty>(string testName, Expression<Func<TInput, TProperty>> propertyExpression, Action<TInput> invalidateAction, string expectedMessage = null, IEnumerable<string> noErrorsFor = null)
        {
            var memberExpression = (MemberExpression)propertyExpression.Body;
            var propertyName = memberExpression.Member.Name;

            return Case(propertyName, invalidateAction, expectedMessage, noErrorsFor).SetName(testName);
        }

        protected static TestCaseData Case(string testName, string propertyName, Action<TInput> invalidateAction, string expectedMessage = null, IEnumerable<string> noErrorsFor = null)
        {
            return Case(propertyName, invalidateAction, expectedMessage, noErrorsFor).SetName(testName);
        }

        private static TestCaseData Case(string propertyName, Action<TInput> invalidateAction, string expectedMessage, IEnumerable<string> noErrorsFor)
        {
            return new(propertyName, invalidateAction, expectedMessage, noErrorsFor);
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Validator = CreateValidator();
        }

        [SetUp]
        public void SetUp()
        {
            Input = CreateInput();
        }

        [TestCaseSource("ValidTestCases")]
        public async Task ValidateAsync_ValidData_ReturnsValidResult(Action<TInput> alterationAction)
        {
            alterationAction(Input);

            ValidationResult result = await Validator.ValidateAsync(Input);

            Assert.That(result.IsValid, Is.True, result.IsValid ? "" : result.Errors[0].ErrorMessage);
        }

        [TestCaseSource("InvalidTestCases")]
        public async Task ValidateAsync_InvalidData_ReturnsInvalidResult(string propertyName, Action<TInput> invalidateAction, string message, string[] noErrorsFor)
        {
            invalidateAction(Input);

            ValidationResult result = await Validator.ValidateAsync(Input);

            Assert.That(result.IsValid, Is.False);

            ValidationFailure[] errors = result.Errors.Where(x => x.PropertyName == propertyName).ToArray();
            Assert.That(errors, Is.Not.Empty);

            if (message != null)
                Assert.That(errors.Select(x => x.ErrorMessage), Has.Member(message));

            if (noErrorsFor != null)
                Assert.That(result.Errors, Has.None.Matches<ValidationFailure>(x => noErrorsFor.Contains(x.PropertyName)));
        }

        protected abstract TInput CreateInput();
        protected abstract TValidator CreateValidator();
    }
}
