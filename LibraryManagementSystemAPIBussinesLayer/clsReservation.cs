using LibraryManagementSystemAPIDataAccessLayer;
using SharedClasses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static SharedClasses.ReservationDTOs;

namespace LibraryManagementSystemAPIBussinesLayer
{
    public static class clsReservation
    {
        public static async Task<Result<ReservationDTO>> FindAsync(int ReservationID)
        {
            if (ReservationID <= 0)
            {
                return new Result<ReservationDTO>(
                    false,
                    new { error = new { header = "Bad Request", body = "The request is invalid. Please check the input and try again." } },
                    null,
                    400
                );
            }
            return await clsReservationData.GetReservationInfoByIDAsync(ReservationID);
        }

        public static async Task<Result<List<ReservationViewDTO>>> GetAllReservationsAsync()
        {
            return await clsReservationData.GetAllReservationsAsync();
        }

        public static Task<Result<bool>> BookCriteriaAsync(clsBook book, clsMember member)
        {
            return Task.FromResult(new Result<bool>(
                true,
                new { success = new { header = "Success", body = "All book criteria met." } },
                true
            ));
        }

        public static async Task<Result<int>> ReserveBookAsync(AddNewReservationDTO addNewReservationDTO)
        {
            if (!IsValidReservationInput(addNewReservationDTO))
            {
                return new Result<int>(
                    false,
                    new { error = new { header = "Bad Request", body = "The request is invalid. Please check the input and try again." } },
                    -1,
                    400
                );
            }

            var memberResult = await clsMember.FindAsync(addNewReservationDTO.MemberID);
            if (!memberResult.Success)
            {
                return new Result<int>(false, memberResult.Message, -1, memberResult.ErrorCode);
            }

            var memberCriteriaResult = await memberResult.Data.CheckMemberCriteriaAsync();
            if (!memberCriteriaResult.Success)
            {
                return new Result<int>(false, memberCriteriaResult.Message, -1, memberCriteriaResult.ErrorCode);
            }

            var bookResult = await clsBook.FindAsync(addNewReservationDTO.BookID);
            if (!bookResult.Success)
            {
                return new Result<int>(false, bookResult.Message, -1, bookResult.ErrorCode);
            }

            if ((await bookResult.Data.IsAvaiableAsync()).Data)
            {
                return new Result<int>(
                    false,
                    new { error = new { header = "Available", body = "The book is available; go ahead and borrow it!" } },
                    -1,
                    422
                );
            }

            if ((await memberResult.Data.DoesHasActiveReservationForBookIDAsync(bookResult.Data.BookID)).Data)
            {
                return new Result<int>(
                    false,
                    new { error = new { header = "Not Allowed", body = "This member already has a reservation for this book!" } },
                    -1,
                    422
                );
            }

            return await clsReservationData.AddNewReservationAsync(addNewReservationDTO);
        }

        private static bool IsValidReservationInput(AddNewReservationDTO dto)
        {
            return dto.MemberID > 0 && dto.BookID > 0 && dto.CreatedByUserID > 0;
        }

        public static async Task<Result<int>> CancelReservationAsync(int reservationID)
        {
            var result = await FindAsync(reservationID);
            if (!result.Success)
            {
                return new Result<int>(
                    false,
                    new { error = new { header = "Not Found", body = "Reservation not found." } },
                    -1,
                    404
                );
            }

            if (result.Data.ReservationStatus == "Cancelled")
            {
                return new Result<int>(
                    false,
                    new { error = new { header = "Already Cancelled", body = "This reservation is already cancelled." } },
                    -1,
                    422
                );
            }

            return await clsReservationData.CancelReservationAsync(reservationID);
        }

    }
}
