using System;
using Xunit;

namespace Kros.Utils.UnitTests.Utils
{
    public class CheckShould
    {
        #region Nested types

        private enum DummyEnum
        {
            Value1,
            Value2,
            Value3,
            Value4,
            Value5
        }

        private class DummyClass : IComparable<DummyClass>
        {
            public int Id { get; set; }
            public string Text { get; set; } = string.Empty;

            public int CompareTo(DummyClass? other)
            {
                return Id.CompareTo(other?.Id);
            }

            public override bool Equals(object? obj)
            {
                if (obj is DummyClass dummy)
                {
                    return dummy.Id == Id;
                }
                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        private class DummyClass2
        {
            public int Id { get; set; }
            public string Text { get; set; } = string.Empty;
        }

        #endregion

        #region Object

        [Fact]
        public void ThrowArgumentNullExceptionForNullableValueTypeWithoutValue()
        {
            int? value = null;
            Action action = () => Check.NotNull(value, nameof(value));
            Assert.Throws<ArgumentNullException>(action);
        }

        [Fact]
        public void NotThrowExceptionForNullableValueTypeWithValue()
        {
            int? value = 123;
            Action action = () => Check.NotNull(value, nameof(value));
            Assert.Null(Record.Exception(action));
        }

        [Fact]
        public void NotThrowExceptionForNullableValueTypeWithDefaultValue()
        {
            int? value = default(int);
            Action action = () => Check.NotNull(value, nameof(value));
            Assert.Null(Record.Exception(action));
        }

        [Fact]
        public void NotThrowExceptionForValueTypeWithDefaultValue()
        {
            int value = default;
            Action action = () => Check.NotNull(value, nameof(value));
            Assert.Null(Record.Exception(action));
        }

        [Fact]
        public void NotThrowExceptionForValueTypeWithValue()
        {
            int value = 123;
            Action action = () => Check.NotNull(value, nameof(value));
            Assert.Null(Record.Exception(action));
        }

        [Fact]
        public void ThrowArgumentNullExceptionForReferenceType()
        {
            string? value = null;
            Action action = () => Check.NotNull(value, nameof(value));
            Assert.Throws<ArgumentNullException>(action);
        }

        [Fact]
        public void ThrowArgumentNullExceptionWithParamName()
        {
            const string paramName = "arg";
            object? value = null;
            Action action = () => Check.NotNull(value, paramName);
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(action);
            Assert.Equal(paramName, ex.ParamName);
        }

        [Fact]
        public void ThrowArgumentNullExceptionWithParamNameAndMessage()
        {
            const string paramName = "arg";
            const string message = "Exception message.";
            object? value = null;
            Action action = () => Check.NotNull(value, paramName, message);
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(action);
            Assert.StartsWith(message, ex.Message);
            Assert.Equal(paramName, ex.ParamName);
        }

        #endregion

        #region Type

        [Fact]
        public void ThrowArgumentExceptionWithParamNameWhenInvalidTypeGeneric()
        {
            const string paramName = "arg";
            DummyClass param = new DummyClass();
            Action action = () => Check.IsOfType<DummyClass2>(param, paramName);
            ArgumentException ex = Assert.Throws<ArgumentException>(action);
            Assert.Contains(typeof(DummyClass2).FullName!, ex.Message);
            Assert.Contains(param.GetType().FullName!, ex.Message);
            Assert.Equal(paramName, ex.ParamName);
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameAndMessageWhenInvalidTypeGeneric()
        {
            const string paramName = "arg";
            const string message = "Exception message.";
            DummyClass param = new DummyClass();
            Action action = () => Check.IsOfType<DummyClass2>(param, paramName, message);
            ArgumentException ex = Assert.Throws<ArgumentException>(action);
            Assert.StartsWith(message, ex.Message);
            Assert.Equal(paramName, ex.ParamName);
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameWhenInvalidType()
        {
            const string paramName = "arg";
            DummyClass param = new DummyClass();
            string expectedTypeName = typeof(DummyClass2).FullName!;
            string argTypeName = param.GetType().FullName!;
            Action action = () => Check.IsOfType(param, typeof(DummyClass2), paramName);
            ArgumentException ex = Assert.Throws<ArgumentException>(action);
            Assert.Contains(typeof(DummyClass2).FullName!, ex.Message);
            Assert.Contains(param.GetType().FullName!, ex.Message);
            Assert.Equal(paramName, ex.ParamName);
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameAndMessageWhenInvalidType()
        {
            const string paramName = "arg";
            const string message = "Exception message.";
            DummyClass param = new DummyClass();
            Action action = () => Check.IsOfType(param, typeof(DummyClass2), paramName, message);
            ArgumentException ex = Assert.Throws<ArgumentException>(action);
            Assert.StartsWith(message, ex.Message);
            Assert.Equal(paramName, ex.ParamName);
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameWhenNotExpectedTypeGeneric()
        {
            const string paramName = "arg";
            DummyClass param = new DummyClass();
            Action action = () => Check.IsNotOfType<DummyClass>(param, paramName);
            ArgumentException ex = Assert.Throws<ArgumentException>(action);
            Assert.Contains(typeof(DummyClass).FullName!, ex.Message);
            Assert.Equal(paramName, ex.ParamName);
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameAndMessageWhenNotExpectedTypeGeneric()
        {
            const string paramName = "arg";
            const string message = "Exception message.";
            DummyClass param = new DummyClass();
            Action action = () => Check.IsNotOfType<DummyClass>(param, paramName, message);
            ArgumentException ex = Assert.Throws<ArgumentException>(action);
            Assert.StartsWith(message, ex.Message);
            Assert.Equal(paramName, ex.ParamName);
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameWhenNotExpectedType()
        {
            const string paramName = "arg";
            DummyClass param = new DummyClass();
            Action action = () => Check.IsNotOfType(param, typeof(DummyClass), paramName);
            ArgumentException ex = Assert.Throws<ArgumentException>(action);
            Assert.Contains(typeof(DummyClass).FullName!, ex.Message);
            Assert.Equal(paramName, ex.ParamName);
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameAndMessageWhenNotExpectedType()
        {
            const string paramName = "arg";
            const string message = "Exception message.";
            DummyClass param = new DummyClass();
            Action action = () => Check.IsNotOfType(param, typeof(DummyClass), paramName, message);
            ArgumentException ex = Assert.Throws<ArgumentException>(action);
            Assert.StartsWith(message, ex.Message);
            Assert.Equal(paramName, ex.ParamName);
        }

        #endregion

        #region String

        [Fact]
        public void ThrowArgumentNullExceptionWithParamNameWhenNullString()
        {
            Action action = () => Check.NotNullOrEmpty(null, "arg");
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(action);
            Assert.Equal("arg", ex.ParamName);
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameWhenEmptyString()
        {
            Action action = () => Check.NotNullOrEmpty(string.Empty, "arg");
            ArgumentException ex = Assert.Throws<ArgumentException>(action);
            Assert.Equal("arg", ex.ParamName);
        }

        [Fact]
        public void ThrowArgumentNullExceptionWithParamNameAndMessageWhenNullString()
        {
            Action action = () => Check.NotNullOrEmpty(null, "arg", "Exception message.");
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(action);
            Assert.StartsWith("Exception message.", ex.Message);
            Assert.Equal("arg", ex.ParamName);
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameAndMessageWhenEmptyString()
        {
            Action action = () => Check.NotNullOrEmpty(string.Empty, "arg", "Exception message.");
            ArgumentException ex = Assert.Throws<ArgumentException>(action);
            Assert.StartsWith("Exception message.", ex.Message);
            Assert.Equal("arg", ex.ParamName);
        }

        [Fact]
        public void ThrowArgumentNullExceptionWithParamNameWhenWhiteSpaceString()
        {
            Action action = () => Check.NotNullOrWhiteSpace(" \t \r \n ", "arg");
            ArgumentException ex = Assert.Throws<ArgumentException>(action);
            Assert.Equal("arg", ex.ParamName);
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameAndMessageWhenWhiteSpaceString()
        {
            Action action = () => Check.NotNullOrWhiteSpace(" \t \r \n ", "arg", "Exception message.");
            ArgumentException ex = Assert.Throws<ArgumentException>(action);
            Assert.StartsWith("Exception message.", ex.Message);
            Assert.Equal("arg", ex.ParamName);
        }

        #endregion

        #region Values

        #region Equal

        [Fact]
        public void ThrowArgumentExceptionWithParamNameWhenPrimitiveValueNotEqual()
        {
            Action action = () => Check.Equal(1, 2, "arg");
            ArgumentException ex = Assert.Throws<ArgumentException>(action);
            Assert.Equal("arg", ex.ParamName);
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameAndMessageWhenPrimitiveValueNotEqual()
        {
            Action action = () => Check.Equal(1, 2, "arg", "Exception message.");
            ArgumentException ex = Assert.Throws<ArgumentException>(action);
            Assert.StartsWith("Exception message.", ex.Message);
            Assert.Equal("arg", ex.ParamName);
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameWhenClassValueNotEqual()
        {
            DummyClass value1 = new DummyClass() { Id = 1 };
            DummyClass value2 = new DummyClass() { Id = 2 };

            Action action = () => Check.Equal(value1, value2, "arg");
            ArgumentException ex = Assert.Throws<ArgumentException>(action);
            Assert.Equal("arg", ex.ParamName);
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameAndMessageWhenClassValueNotEqual()
        {
            DummyClass value1 = new DummyClass() { Id = 1 };
            DummyClass value2 = new DummyClass() { Id = 2 };

            Action action = () => Check.Equal(value1, value2, "arg", "Exception message.");
            ArgumentException ex = Assert.Throws<ArgumentException>(action);
            Assert.StartsWith("Exception message.", ex.Message);
            Assert.Equal("arg", ex.ParamName);
        }

        #endregion

        #region Not equal

        [Fact]
        public void ThrowArgumentExceptionWithParamNameWhenPrimitiveValueEquals()
        {
            Action action = () => Check.NotEqual(2, 2, "arg");
            ArgumentException ex = Assert.Throws<ArgumentException>(action);
            Assert.Equal("arg", ex.ParamName);
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameAndMessageWhenPrimitiveValueEquals()
        {
            Action action = () => Check.NotEqual(2, 2, "arg", "Exception message.");
            ArgumentException ex = Assert.Throws<ArgumentException>(action);
            Assert.StartsWith("Exception message.", ex.Message);
            Assert.Equal("arg", ex.ParamName);
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameWhenClassValueEquals()
        {
            DummyClass value1 = new DummyClass() { Id = 2 };
            DummyClass value2 = new DummyClass() { Id = 2 };

            Action action = () => Check.NotEqual(value1, value2, "arg");
            ArgumentException ex = Assert.Throws<ArgumentException>(action);
            Assert.Equal("arg", ex.ParamName);
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameAndMessageWhenClassValueEquals()
        {
            DummyClass value1 = new DummyClass() { Id = 2 };
            DummyClass value2 = new DummyClass() { Id = 2 };

            Action action = () => Check.NotEqual(value1, value2, "arg", "Exception message.");
            ArgumentException ex = Assert.Throws<ArgumentException>(action);
            Assert.StartsWith("Exception message.", ex.Message);
            Assert.Equal("arg", ex.ParamName);
        }

        #endregion

        #region LessThan

        [Fact]
        public void ThrowArgumentExceptionWithParamNameWhenPrimitiveValueNotLessThanExpected()
        {
            Action action = () => Check.LessThan(1, 1, "arg");
            ArgumentException ex = Assert.Throws<ArgumentException>(action);
            Assert.Equal("arg", ex.ParamName);

            action = () => Check.LessThan(2, 1, "arg");
            ex = Assert.Throws<ArgumentException>(action);
            Assert.Equal("arg", ex.ParamName);
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameAndMessageWhenPrimitiveValueNotLessThanExpected()
        {
            Action action = () => Check.LessThan(1, 1, "arg", "Exception message.");
            ArgumentException ex = Assert.Throws<ArgumentException>(action);
            Assert.StartsWith("Exception message.", ex.Message);
            Assert.Equal("arg", ex.ParamName);

            action = () => Check.LessThan(2, 1, "arg", "Exception message.");
            ex = Assert.Throws<ArgumentException>(action);
            Assert.StartsWith("Exception message.", ex.Message);
            Assert.Equal("arg", ex.ParamName);
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameWhenClassValueNotLessThanExpected()
        {
            DummyClass value1a = new DummyClass() { Id = 1 };
            DummyClass value1b = new DummyClass() { Id = 1 };
            DummyClass value2 = new DummyClass() { Id = 2 };

            Action action = () => Check.LessThan(value1a, value1b, "arg");
            ArgumentException ex = Assert.Throws<ArgumentException>(action);
            Assert.Equal("arg", ex.ParamName);

            action = () => Check.LessThan(value2, value1a, "arg");
            ex = Assert.Throws<ArgumentException>(action);
            Assert.Equal("arg", ex.ParamName);
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameAndMessageWhenClassValueNotLessThanExpected()
        {
            DummyClass value1a = new DummyClass() { Id = 1 };
            DummyClass value1b = new DummyClass() { Id = 1 };
            DummyClass value2 = new DummyClass() { Id = 2 };

            Action action = () => Check.LessThan(value1a, value1b, "arg", "Exception message.");
            ArgumentException ex = Assert.Throws<ArgumentException>(action);
            Assert.StartsWith("Exception message.", ex.Message);
            Assert.Equal("arg", ex.ParamName);

            action = () => Check.LessThan(value2, value1a, "arg", "Exception message.");
            ex = Assert.Throws<ArgumentException>(action);
            Assert.StartsWith("Exception message.", ex.Message);
            Assert.Equal("arg", ex.ParamName);
        }

        #endregion

        #region LessOrEqualThan

        [Fact]
        public void ThrowArgumentExceptionWithParamNameWhenPrimitiveValueNotLessOrEqualThanExpected()
        {
            Action action = () => Check.LessOrEqualThan(1, 1, "arg");
            Assert.Null(Record.Exception(action));

            action = () => Check.LessOrEqualThan(2, 1, "arg");
            ArgumentException ex = Assert.Throws<ArgumentException>(action);
            Assert.Equal("arg", ex.ParamName);
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameAndMessageWhenPrimitiveValueNotLessOrEqualThanExpected()
        {
            Action action = () => Check.LessOrEqualThan(1, 1, "arg", "Exception message.");
            Assert.Null(Record.Exception(action));

            action = () => Check.LessOrEqualThan(2, 1, "arg", "Exception message.");
            ArgumentException ex = Assert.Throws<ArgumentException>(action);
            Assert.StartsWith("Exception message.", ex.Message);
            Assert.Equal("arg", ex.ParamName);
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameWhenClassValueNotLessOrEqualThanExpected()
        {
            DummyClass value1a = new DummyClass() { Id = 1 };
            DummyClass value1b = new DummyClass() { Id = 1 };
            DummyClass value2 = new DummyClass() { Id = 2 };

            Action action = () => Check.LessOrEqualThan(value1a, value1b, "arg");
            Assert.Null(Record.Exception(action));

            action = () => Check.LessOrEqualThan(value2, value1a, "arg");
            ArgumentException ex = Assert.Throws<ArgumentException>(action);
            Assert.Equal("arg", ex.ParamName);
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameAndMessageWhenClassValueNotLessOrEqualThanExpected()
        {
            DummyClass value1a = new DummyClass() { Id = 1 };
            DummyClass value1b = new DummyClass() { Id = 1 };
            DummyClass value2 = new DummyClass() { Id = 2 };

            Action action = () => Check.LessOrEqualThan(value1a, value1b, "arg", "Exception message.");
            Assert.Null(Record.Exception(action));

            action = () => Check.LessOrEqualThan(value2, value1a, "arg", "Exception message.");
            ArgumentException ex = Assert.Throws<ArgumentException>(action);
            Assert.StartsWith("Exception message.", ex.Message);
            Assert.Equal("arg", ex.ParamName);
        }

        #endregion

        #region GreaterThan

        [Fact]
        public void ThrowArgumentExceptionWithParamNameWhenPrimitiveValueNotGreaterThanExpected()
        {
            Action action = () => Check.GreaterThan(1, 1, "arg");
            ArgumentException ex = Assert.Throws<ArgumentException>(action);
            Assert.Equal("arg", ex.ParamName);

            action = () => Check.GreaterThan(1, 2, "arg");
            ex = Assert.Throws<ArgumentException>(action);
            Assert.Equal("arg", ex.ParamName);
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameAndMessageWhenPrimitiveValueNotGreaterThanExpected()
        {
            Action action = () => Check.GreaterThan(1, 1, "arg", "Exception message.");
            ArgumentException ex = Assert.Throws<ArgumentException>(action);
            Assert.StartsWith("Exception message.", ex.Message);
            Assert.Equal("arg", ex.ParamName);

            action = () => Check.GreaterThan(1, 2, "arg", "Exception message.");
            ex = Assert.Throws<ArgumentException>(action);
            Assert.StartsWith("Exception message.", ex.Message);
            Assert.Equal("arg", ex.ParamName);
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameWhenClassValueNotGreaterThanExpected()
        {
            DummyClass value1a = new DummyClass() { Id = 1 };
            DummyClass value1b = new DummyClass() { Id = 1 };
            DummyClass value2 = new DummyClass() { Id = 2 };

            Action action = () => Check.GreaterThan(value1a, value1b, "arg");
            ArgumentException ex = Assert.Throws<ArgumentException>(action);
            Assert.Equal("arg", ex.ParamName);

            action = () => Check.GreaterThan(value1a, value2, "arg");
            ex = Assert.Throws<ArgumentException>(action);
            Assert.Equal("arg", ex.ParamName);
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameAndMessageWhenClassValueNotGreaterThanExpected()
        {
            DummyClass value1a = new DummyClass() { Id = 1 };
            DummyClass value1b = new DummyClass() { Id = 1 };
            DummyClass value2 = new DummyClass() { Id = 2 };

            Action action = () => Check.GreaterThan(value1a, value1b, "arg", "Exception message.");
            ArgumentException ex = Assert.Throws<ArgumentException>(action);
            Assert.StartsWith("Exception message.", ex.Message);
            Assert.Equal("arg", ex.ParamName);

            action = () => Check.GreaterThan(value1a, value2, "arg", "Exception message.");
            ex = Assert.Throws<ArgumentException>(action);
            Assert.StartsWith("Exception message.", ex.Message);
            Assert.Equal("arg", ex.ParamName);
        }

        #endregion

        #region GreaterOrEqualThan

        [Fact]
        public void ThrowArgumentExceptionWithParamNameWhenPrimitiveValueNotGreaterOrEqualThanExpected()
        {
            Action action = () => Check.GreaterOrEqualThan(1, 1, "arg");
            Assert.Null(Record.Exception(action));

            action = () => Check.GreaterOrEqualThan(1, 2, "arg");
            ArgumentException ex = Assert.Throws<ArgumentException>(action);
            Assert.Equal("arg", ex.ParamName);
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameAndMessageWhenPrimitiveValueNotGreaterOrEqualThanExpected()
        {
            Action action = () => Check.GreaterOrEqualThan(1, 1, "arg", "Exception message.");
            Assert.Null(Record.Exception(action));

            action = () => Check.GreaterOrEqualThan(1, 2, "arg", "Exception message.");
            ArgumentException ex = Assert.Throws<ArgumentException>(action);
            Assert.StartsWith("Exception message.", ex.Message);
            Assert.Equal("arg", ex.ParamName);
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameWhenClassValueNotGreaterOrEqualThanExpected()
        {
            DummyClass value1a = new DummyClass() { Id = 1 };
            DummyClass value1b = new DummyClass() { Id = 1 };
            DummyClass value2 = new DummyClass() { Id = 2 };

            Action action = () => Check.GreaterOrEqualThan(value1a, value1b, "arg");
            Assert.Null(Record.Exception(action));

            action = () => Check.GreaterOrEqualThan(value1a, value2, "arg");
            ArgumentException ex = Assert.Throws<ArgumentException>(action);
            Assert.Equal("arg", ex.ParamName);
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameAndMessageWhenClassValueNotGreaterOrEqualThanExpected()
        {
            DummyClass value1a = new DummyClass() { Id = 1 };
            DummyClass value1b = new DummyClass() { Id = 1 };
            DummyClass value2 = new DummyClass() { Id = 2 };

            Action action = () => Check.GreaterOrEqualThan(value1a, value1b, "arg", "Exception message.");
            Assert.Null(Record.Exception(action));

            action = () => Check.GreaterOrEqualThan(value1a, value2, "arg", "Exception message.");
            ArgumentException ex = Assert.Throws<ArgumentException>(action);
            Assert.StartsWith("Exception message.", ex.Message);
            Assert.Equal("arg", ex.ParamName);
        }

        #endregion

        #region IsInList

        [Fact]
        public void ThrowArgumentExceptionWhenEnumArgumentIsNotInListWithParamName()
        {
            DummyEnum value = DummyEnum.Value1;
            Action action = () => Check.IsInList(value, new DummyEnum[] { DummyEnum.Value2, DummyEnum.Value3 }, "arg");
            ArgumentException ex = Assert.Throws<ArgumentException>(action);
            Assert.Contains("Value1", ex.Message);
            Assert.Contains("Value2, Value3", ex.Message);
            Assert.Equal("arg", ex.ParamName);
        }

        [Fact]
        public void ThrowArgumentExceptionWhenEnumArgumentIsNotInListWithParamNameAndCustomMessage()
        {
            DummyEnum value = DummyEnum.Value1;
            Action action = () => Check.IsInList(value, new DummyEnum[] { DummyEnum.Value2, DummyEnum.Value3 }, "arg", "msg");
            ArgumentException ex = Assert.Throws<ArgumentException>(action);
            Assert.StartsWith("msg", ex.Message);
            Assert.Equal("arg", ex.ParamName);
        }

        [Fact]
        public void NotThrowArgumentExceptionWhenEnumArgumentIsInListWithParamName()
        {
            DummyEnum value = DummyEnum.Value3;
            Action action = () => Check.IsInList(value, new DummyEnum[] { DummyEnum.Value2, DummyEnum.Value3 }, "arg");
            Assert.Null(Record.Exception(action));
        }

        [Fact]
        public void NotThrowArgumentExceptionWhenEnumArgumentIsInListWithParamNameAndCustomMessage()
        {
            DummyEnum value = DummyEnum.Value3;
            Action action = () => Check.IsInList(value, new DummyEnum[] { DummyEnum.Value2, DummyEnum.Value3 }, "arg", "msg");
            Assert.Null(Record.Exception(action));
        }

        [Fact]
        public void ThrowArgumentExceptionWhenStringArgumentIsNotInListWithParamName()
        {
            string value = "a";
            Action action = () => Check.IsInList(value, new string[] { "b", "c", "d" }, "arg");
            ArgumentException ex = Assert.Throws<ArgumentException>(action);
            Assert.Contains("a", ex.Message);
            Assert.Contains("b, c, d", ex.Message);
            Assert.Equal("arg", ex.ParamName);
        }

        [Fact]
        public void ThrowArgumentExceptionWhenStringArgumentIsNotInListWithParamNameAndCustomMessage()
        {
            string value = "a";
            Action action = () => Check.IsInList(value, new string[] { "b", "c", "d" }, "arg", "msg");
            ArgumentException ex = Assert.Throws<ArgumentException>(action);
            Assert.StartsWith("msg", ex.Message);
            Assert.Equal("arg", ex.ParamName);
        }

        [Fact]
        public void NotThrowArgumentExceptionWhenStringArgumentIsInListWithParamName()
        {
            string value = "d";
            Action action = () => Check.IsInList(value, new string[] { "b", "c", "d" }, "arg");
            Assert.Null(Record.Exception(action));
        }

        [Fact]
        public void NotThrowArgumentExceptionWhenStringArgumentIsInListWithParamNameAndCustomMessage()
        {
            string value = "d";
            Action action = () => Check.IsInList(value, new string[] { "b", "c", "d" }, "arg", "msg");
            Assert.Null(Record.Exception(action));
        }

        #endregion

        #region IsNotInList

        [Fact]
        public void ThrowArgumentExceptionWhenEnumArgumentIsInListWithParamName()
        {
            DummyEnum value = DummyEnum.Value1;
            Action action = () => Check.IsNotInList(value, new DummyEnum[] { DummyEnum.Value1, DummyEnum.Value2 }, "arg");
            ArgumentException ex = Assert.Throws<ArgumentException>(action);
            Assert.Contains("Value1", ex.Message);
            Assert.Contains("Value1, Value2", ex.Message);
            Assert.Equal("arg", ex.ParamName);
        }

        [Fact]
        public void ThrowArgumentExceptionWhenEnumArgumentIsInListWithParamNameAndCustomMessage()
        {
            DummyEnum value = DummyEnum.Value1;
            Action action = () => Check.IsNotInList(value, new DummyEnum[] { DummyEnum.Value1, DummyEnum.Value2 }, "arg", "msg");
            ArgumentException ex = Assert.Throws<ArgumentException>(action);
            Assert.StartsWith("msg", ex.Message);
            Assert.Equal("arg", ex.ParamName);
        }

        [Fact]
        public void NotThrowArgumentExceptionWhenEnumArgumentIsNotInListWithParamName()
        {
            DummyEnum value = DummyEnum.Value3;
            Action action = () => Check.IsNotInList(value, new DummyEnum[] { DummyEnum.Value1, DummyEnum.Value2 }, "arg");
            Assert.Null(Record.Exception(action));
        }

        [Fact]
        public void NotThrowArgumentExceptionWhenEnumArgumentIsNotInListWithParamNameAndCustomMessage()
        {
            DummyEnum value = DummyEnum.Value3;
            Action action = () => Check.IsNotInList(value, new DummyEnum[] { DummyEnum.Value1, DummyEnum.Value2 }, "arg", "msg");
            Assert.Null(Record.Exception(action));
        }

        [Fact]
        public void ThrowArgumentExceptionWhenStringArgumentIsInListWithParamName()
        {
            string value = "a";
            Action action = () => Check.IsNotInList(value, new string[] { "a", "b", "c" }, "arg");
            ArgumentException ex = Assert.Throws<ArgumentException>(action);
            Assert.Contains("a", ex.Message);
            Assert.Contains("a, b, c", ex.Message);
            Assert.Equal("arg", ex.ParamName);
        }

        [Fact]
        public void ThrowArgumentExceptionWhenStringArgumentIsInListWithParamNameAndCustomMessage()
        {
            string value = "a";
            Action action = () => Check.IsNotInList(value, new string[] { "a", "b", "c" }, "arg", "msg");
            ArgumentException ex = Assert.Throws<ArgumentException>(action);
            Assert.StartsWith("msg", ex.Message);
            Assert.Equal("arg", ex.ParamName);
        }

        [Fact]
        public void NotThrowArgumentExceptionWhenStringArgumentIsNotInListWithParamName()
        {
            string value = "d";
            Action action = () => Check.IsNotInList(value, new string[] { "a", "b", "c" }, "arg");
            Assert.Null(Record.Exception(action));
        }

        [Fact]
        public void NotThrowArgumentExceptionWhenStringArgumentIsNotInListWithParamNameAndCustomMessage()
        {
            string value = "d";
            Action action = () => Check.IsNotInList(value, new string[] { "a", "b", "c" }, "arg", "msg");
            Assert.Null(Record.Exception(action));
        }

        #endregion

        #endregion

        #region Guid

        [Fact]
        public void ThrowArgumentExceptionWithParamNameWhenEmptyGuid()
        {
            Guid value = Guid.Empty;
            Action action = () => Check.NotEmptyGuid(value, "arg");
            ArgumentException ex = Assert.Throws<ArgumentException>(action);
            Assert.Equal("arg", ex.ParamName);
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameAndMessageWhenEmptyGuid()
        {
            Guid value = Guid.Empty;
            Action action = () => Check.NotEmptyGuid(value, "arg", "Exception message.");
            ArgumentException ex = Assert.Throws<ArgumentException>(action);
            Assert.StartsWith("Exception message.", ex.Message);
            Assert.Equal("arg", ex.ParamName);
        }

        #endregion
    }
}
