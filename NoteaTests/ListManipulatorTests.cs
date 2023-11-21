using Microsoft.EntityFrameworkCore;
using Moq;
using NOTEA.Database;
using NOTEA.Models.ConspectModels;
using NOTEA.Repositories.GenericRepositories;
using NOTEA.Services.LogServices;
using NOTEA.Utilities.ListManipulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NoteaTests
{
    public class ListManipulatorTests
    {
        [Fact]
        public void UpdateFilter_WhenSearchByIsNullOrEmpty_ShouldSetDefaults()
        {
            var listManipulator = new ListManipulator();
            listManipulator.UpdateFilter("", "searchValue");
            Assert.False(listManipulator.FilterExists);
            Assert.Equal("name", listManipulator.SearchBy);
            Assert.Equal("searchValue", listManipulator.SearchValue);
        }

        [Fact]
        public void UpdateFilter_WhenSearchByIsNotNullOrEmpty_ShouldSetValues()
        {
            var listManipulator = new ListManipulator();
            listManipulator.UpdateFilter("customField", "searchValue");

            Assert.True(listManipulator.FilterExists);
            Assert.Equal("customField", listManipulator.SearchBy);
            Assert.Equal("searchValue", listManipulator.SearchValue);
        }
        [Fact]
        public void GetSelection_ShouldReturnNonNullFunction_WhenTempIsNotNull()
        {
            var listManipulator = new ListManipulator();
            listManipulator.UpdateFilter("name", "searchValue");
            var result = listManipulator.GetSelection();
            Assert.NotNull(result);
        }

        [Fact]
        public void GenerateFilter_ShouldReturnFilterFunction_WhenSearchValueIsNotNullOrEmpty()
        {
            var listManipulator = new ListManipulator();
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
            var listManipulator = new ListManipulator();
            var result = listManipulator.GetSelection();
            Assert.Null(result);
        }
        [Fact]
        public void GenerateFilter_ShouldReturnNullFunction_WhenSearchValueIsNullOrEmpty()
        {
            var listManipulator = new ListManipulator();
            string searchBy = "name";
            string searchValue = null;  // or an empty string
            var filterFunction = listManipulator.GenerateFilter(searchBy, searchValue);
            Assert.Null(filterFunction);
        }
        [Fact]
        public void GenerateFilter_ShouldReturnNullFunction_WhenSearchByIsInvalid()
        {
            var listManipulator = new ListManipulator();
            string searchBy = "invalidField";
            string searchValue = "example";
            var filterFunction = listManipulator.GenerateFilter(searchBy, searchValue);
            Assert.Null(filterFunction);
        }

        [Fact]
        public void UpdateSort_ShouldUpdateSortStatus_WhenValidSortColumnIsProvided()
        {
            var listManipulator = new ListManipulator();
            var initialSortStatus = listManipulator.SortStatus.ToArray();

            listManipulator.UpdateSort(SortCollumn.Semester);

            Assert.NotEqual(initialSortStatus, listManipulator.SortStatus);
            Assert.Equal(SortPhase.Ascending, listManipulator.SortStatus[(int)SortCollumn.Semester]);
            Assert.Equal(SortPhase.None, listManipulator.SortStatus[(int)SortCollumn.Name]);
            Assert.Equal(SortPhase.None, listManipulator.SortStatus[(int)SortCollumn.Date]);
        }

        [Fact]
        public void ClearFilter_ShouldResetFilterProperties()
        {
            var listManipulator = new ListManipulator();
            listManipulator.UpdateFilter("customField", "searchValue");

            listManipulator.ClearFilter();

            Assert.False(listManipulator.FilterExists);
            Assert.Equal("name", listManipulator.SearchBy);
            Assert.Null(listManipulator.SearchValue);
        }

    }
}
