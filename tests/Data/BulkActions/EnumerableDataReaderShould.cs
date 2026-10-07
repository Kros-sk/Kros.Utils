using Kros.Data.BulkActions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Kros.Utils.UnitTests.Data.BulkActions
{
    public class EnumerableDataReaderShould
    {
        #region Nested types

        private class DataItem
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public double Value { get; set; }
        }

        #endregion

        #region Tests

        [Fact]
        public void ThrowArgumentNullExceptionWhenDataIsNull()
        {
            Action createInstance = () =>
            {
                var instance = new EnumerableDataReader<DataItem>(null!, new string[] { "Id" });
            };
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(createInstance);
            Assert.Equal("data", ex.ParamName);
        }

        [Fact]
        public void ThrowArgumentNullExceptionWhenColumnNamesIsNull()
        {
            Action createInstance = () =>
            {
                var instance = new EnumerableDataReader<DataItem>(new List<DataItem>(), null!);
            };
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(createInstance);
            Assert.Equal("columnNames", ex.ParamName);
        }

        [Fact]
        public void ThrowArgumentExceptionWhenColumnNamesIsEmpty()
        {
            Action createInstance = () =>
            {
                var instance = new EnumerableDataReader<DataItem>(new List<DataItem>(), new string[] { });
            };
            ArgumentException ex = Assert.Throws<ArgumentException>(createInstance);
            Assert.Equal("columnNames", ex.ParamName);
        }

        [Fact]
        public void ThrowInvalidOperationExceptionWhenColumnNameIsInvalid()
        {
            const string invalidColumn = "Lorem";

            Action createInstance = () =>
            {
                var instance = new EnumerableDataReader<DataItem>(new List<DataItem>(), new string[] { "Id", invalidColumn });
            };
            InvalidOperationException ex = Assert.Throws<InvalidOperationException>(createInstance);
            Assert.Contains(typeof(DataItem).FullName!, ex.Message);
            Assert.Contains(invalidColumn, ex.Message);
        }

        [Fact]
        public void CreateInstanceCorrectly()
        {
            var reader = new EnumerableDataReader<DataItem>(new List<DataItem>(), new string[] { "Id", "Name" });
            Assert.Equal(0, reader.GetOrdinal("Id"));
            Assert.Equal(1, reader.GetOrdinal("Name"));
            Assert.Equal("Id", reader.GetName(0));
            Assert.Equal("Name", reader.GetName(1));
        }

        [Fact]
        public void ReturnCorrectData()
        {
            var item1 = new DataItem() { Id = 1, Name = "Item 1" };
            var item2 = new DataItem() { Id = 2, Name = "Item 2" };
            var data = new List<DataItem>(new DataItem[] { item1, item2 });

            using (var reader = new EnumerableDataReader<DataItem>(new List<DataItem>(), new string[] { "Id", "Name" }))
            {
                int itemIndex = 0;
                while (reader.Read())
                {
                    Assert.Equal(data[itemIndex].Id, reader.GetValue(0));
                    Assert.Equal(data[itemIndex].Name, reader.GetValue(1));
                    itemIndex++;
                }
            }
        }

        #endregion
    }
}
