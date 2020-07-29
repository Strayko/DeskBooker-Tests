using DeskBooker.Web.Pages;
using System;
using System.Collections.Generic;
using System.Text;
using DeskBooker.Core.Domain;
using DeskBooker.Core.Processor;
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
    }
}
