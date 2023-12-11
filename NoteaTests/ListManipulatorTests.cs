using Microsoft.EntityFrameworkCore;
using Moq;
using NOTEA.Models.ConspectModels;
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
        ListManipulator listManipulator = new ListManipulator();
        [Fact]
        public void UpdateFilter_WhenSearchByIsNullOrEmpty_ShouldSetDefaults()
        {
            listManipulator.UpdateFilter("", "searchValue");
            Assert.False(listManipulator.FilterExists);
            Assert.Equal("name", listManipulator.SearchBy);
            Assert.Equal("searchValue", listManipulator.SearchValue);
        }

        [Fact]
        public void UpdateFilter_WhenSearchByIsNotNullOrEmpty_ShouldSetValues()
        {
            listManipulator.UpdateFilter("customField", "searchValue");

            Assert.True(listManipulator.FilterExists);
            Assert.Equal("customField", listManipulator.SearchBy);
            Assert.Equal("searchValue", listManipulator.SearchValue);
        }
        [Fact]
        public void GetSelection_ShouldReturnNonNullFunction_WhenTempIsNotNull()
        {
            listManipulator.UpdateFilter("name", "searchValue");
            var result = listManipulator.GetSelection();
            Assert.NotNull(result);
        }

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
        public void UpdateSort_ShouldUpdateSortStatus_WhenValidSortColumnIsProvided()
        {
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
            listManipulator.UpdateFilter("customField", "searchValue");

            listManipulator.ClearFilter();

            Assert.False(listManipulator.FilterExists);
            Assert.Equal("name", listManipulator.SearchBy);
            Assert.Null(listManipulator.SearchValue);
        }

    }
}
