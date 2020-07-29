using DeskBooker.Web.Pages;
using System;
using System.Collections.Generic;
using System.Text;
using DeskBooker.Core.Domain;
using DeskBooker.Core.Processor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Xunit;
using Moq;

namespace DeskBooker.Web.Tests
{
    public class BookDeskModelTests
    {
        private Mock<IDeskBookingRequestProcessor> _processorMock;
        private BookDeskModel _bookDeskModel;
        private DeskBookingResult _deskBookintResult;

        public BookDeskModelTests()
        {
            _processorMock = new Mock<IDeskBookingRequestProcessor>();

            _bookDeskModel = new BookDeskModel(_processorMock.Object)
            {
                DeskBookingRequest = new DeskBookingRequest()
            };

            _deskBookintResult = new DeskBookingResult
            {
                Code = DeskBookingResultCode.Success
            };

            _processorMock.Setup(x => x.BookDesk(_bookDeskModel.DeskBookingRequest)).Returns(_deskBookintResult);
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(0, false)]
        public void ShouldCallBookDeskMethodOfProcessorIfModelIsValid(int expectedBookDeskCalls, bool isModelValid)
        {
            if (!isModelValid)
            {
                _bookDeskModel.ModelState.AddModelError("JustAKey", "AnErrorMessage");
            }

            // Act
            _bookDeskModel.OnPost();

            // Assert
            _processorMock.Verify(x => x.BookDesk(_bookDeskModel.DeskBookingRequest), Times.Exactly(expectedBookDeskCalls));
        }

        [Fact]
        public void ShouldAddModelErrorIfNoDeskIsAvailable()
        {
            _deskBookintResult.Code = DeskBookingResultCode.NoDeskAvailable;

            // Act
            _bookDeskModel.OnPost();

            // Assert
            var modelStateEntry = Assert.Contains("DeskBookingRequest.Date", _bookDeskModel.ModelState);
            var modelError = Assert.Single(modelStateEntry.Errors);
            Assert.Equal("No desk available for selected date", modelError.ErrorMessage);
        }

        [Fact]
        public void ShouldNotAddModelErrorIfDeskIsAvailable()
        {
            _deskBookintResult.Code = DeskBookingResultCode.Success;

            _bookDeskModel.OnPost();

            Assert.DoesNotContain("DeskBookingRequest.Date", _bookDeskModel.ModelState);
        }

        [Theory]
        [InlineData(typeof(PageResult), false, null)]
        [InlineData(typeof(PageResult), true, DeskBookingResultCode.NoDeskAvailable)]
        [InlineData(typeof(RedirectToPageResult), true, DeskBookingResultCode.Success)]
        public void ShouldReturnExpectedActionResult(Type expectedActionResult, bool isModelValid,
            DeskBookingResultCode? deskBookingResultCode)
        {
            // Arrange
            if (!isModelValid)
            {
                _bookDeskModel.ModelState.AddModelError("JustAKey", "AnErrorMessage");
            }

            if (deskBookingResultCode.HasValue)
            {
                _deskBookintResult.Code = deskBookingResultCode.Value;
            }

            // Act
            IActionResult actionResult = _bookDeskModel.OnPost();

            // Assert
            Assert.IsType(expectedActionResult, actionResult);
        }

        [Fact]
        public void ShouldRedirectToBookDeskConfirmationPage()
        {
            // Arrange
            _deskBookintResult.Code = DeskBookingResultCode.Success;
            _deskBookintResult.DeskBookingId = 7;
            _deskBookintResult.FirstName = "Moamer";
            _deskBookintResult.Date = new DateTime(2020, 1, 28);

            // Act
            IActionResult actionResult = _bookDeskModel.OnPost();

            // Assert
            var redirectToPageResult = Assert.IsType<RedirectToPageResult>(actionResult);
            Assert.Equal("BookDeskConfirmation", redirectToPageResult.PageName);

            IDictionary<string, object> routeValues = redirectToPageResult.RouteValues;
            Assert.Equal(3, routeValues.Count);

            var deskBookingId = Assert.Contains("DeskBookingId", routeValues);
            Assert.Equal(_deskBookintResult.DeskBookingId, deskBookingId);

            var firstName = Assert.Contains("FirstName", routeValues);
            Assert.Equal(_deskBookintResult.FirstName, firstName);

            var date = Assert.Contains("Date", routeValues);
            Assert.Equal(_deskBookintResult.Date, date);
        }
    }
}
