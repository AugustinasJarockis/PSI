using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Testing;
using NOTEA.Extentions;
using NOTEA.Models.ConspectModels;

namespace NoteaTests
{
    public class ExtensionTests
    {
        [Fact]
        public void IsValidName_InputIsValid_ReturnTrue()
        {
            string name = "konspektas";
            Assert.True(name.IsValidName());
        }
        [Fact]
        public void IsValidName_InputIsInvalid_ReturnTrue()
        {
            string name = "/.konspektas";
            Assert.False(name.IsValidName());
        }
        [Fact]
        public void IsValidEmail_InputIsInvalid_ReturnFalse()
        {
            string name = "aa";
            Assert.False(name.IsValidEmail());
        }
        [Fact]
        public void IsValidEmail_InputIsValid_ReturnTrue()
        {
            string name = "aa@gmail.com";
            Assert.True(name.IsValidEmail());
        }
        [Fact]
        public void GetSelectList_ReturnCorrectItems()
        {
            var expectedItems = new List<SelectListItem>
            {
                new SelectListItem { Text = "Unknown", Value = "Unknown" },
                new SelectListItem { Text = "2022 autumn", Value = "Autumn_2022" },
                new SelectListItem { Text = "2023 spring", Value = "Spring_2023" },
                new SelectListItem { Text = "2023 autumn", Value = "Autumn_2023" },
                new SelectListItem { Text = "2024 spring", Value = "Spring_2024" },
                new SelectListItem { Text = "2024 autumn", Value = "Autumn_2024" }
            };
            var actualItems = EnumExtension.GetSelectList<ConspectSemester>().ToList();

            Assert.Equal(expectedItems.Count, actualItems.Count);

            for (int i = 0; i < expectedItems.Count; i++)
            {
                Assert.Equal(expectedItems[i].Text, actualItems[i].Text);
                Assert.Equal(expectedItems[i].Value, actualItems[i].Value);
            }
        }
        [Fact]
        public void GetSelectList_ReturnInCorrectItems()
        {
            var expectedItems = new List<SelectListItem>
            {
                new SelectListItem { Text = "Known", Value = "Known" },
                new SelectListItem { Text = "Test", Value = "Test" },
                new SelectListItem { Text = "Test", Value = "Test" },
                new SelectListItem { Text = "Test", Value = "Test" },
                new SelectListItem { Text = "Test", Value = "Test" },
                new SelectListItem { Text = "Test", Value = "Test" }
            };
            var actualItems = EnumExtension.GetSelectList<ConspectSemester>().ToList();

            Assert.Equal(expectedItems.Count, actualItems.Count);

            for (int i = 0; i < expectedItems.Count; i++)
            {
                Assert.NotEqual(expectedItems[i].Text, actualItems[i].Text);
                Assert.NotEqual(expectedItems[i].Value, actualItems[i].Value);
            }
        }
        [Fact]
        public void GetDisplayName_ReturnsCorrectDisplayName()
        {
            var enumValue = ConspectSemester.Autumn_2023;
            var displayName = enumValue.GetDisplayName();
            Assert.Equal("2023 autumn", displayName);
        }
    }
}