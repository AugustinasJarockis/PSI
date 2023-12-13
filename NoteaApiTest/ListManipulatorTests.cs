using NoteaAPI.Models.ConspectModels;
using NoteaAPI.Utilities.ListManipulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteaApiUnitTest
{
    public class ListManipulatorTests
    {
        ListManipulator listManipulator = new ListManipulator();
        [Fact]
        public void GenerateFilter_ShouldReturnFilterFunction_WhenSearchValueIsNotNullOrEmpty()
        {
            string searchBy = "name";
            string searchValue = "example";
            var list = new List<ConspectModel> { new ConspectModel { Name = "Example" } };
            var filterFunction = listManipulator.GenerateFilter(searchBy, searchValue);
            Assert.NotNull(filterFunction);
            var filteredList = filterFunction(list);
            Assert.Single(filteredList);
            Assert.Equal("Example", filteredList.First().Name);
        }
        [Fact]
        public void GetSelection_ShouldReturnNullFunction_WhenFilterDoesNotExist()
        {
            var result = listManipulator.GetSelection();
            Assert.Null(result);
        }
        [Fact]
        public void GenerateFilter_ShouldReturnNullFunction_WhenSearchValueIsNullOrEmpty()
        {
            string searchBy = "name";
            string searchValue = null;
            var filterFunction = listManipulator.GenerateFilter(searchBy, searchValue);
            Assert.Null(filterFunction);
        }
        [Fact]
        public void GenerateFilter_ShouldReturnNullFunction_WhenSearchByIsInvalid()
        {
            string searchBy = "invalidField";
            string searchValue = "example";
            var filterFunction = listManipulator.GenerateFilter(searchBy, searchValue);
            Assert.Null(filterFunction);
        }
        [Fact]
        public void GenerateFilter_WhenSearchValueIsNullOrEmpty_ShouldReturnNull()
        {
            var listManipulator = new ListManipulator();

            var filter = listManipulator.GenerateFilter("name", null);

            Assert.Null(filter);
        }

        [Fact]
        public void GenerateFilter_WhenSearchByIsName_ShouldReturnFilterByName()
        {
            var listManipulator = new ListManipulator();

            var filter = listManipulator.GenerateFilter("name", "searchValue");

            Assert.NotNull(filter);
        }

        [Fact]
        public void GenerateSort_WhenCollumnIsNull_ShouldReturnNull()
        {
            var listManipulator = new ListManipulator();

            var sort = listManipulator.GenerateSort(null);

            Assert.Null(sort);
        }

    }
}
