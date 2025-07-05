using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
  
namespace SharedClasses
{
    public class Result<T>
    {
        public bool Success { get; set; }
        public  string Message { get; set; }
        public int ErrorCode { get; set; }
        public T? Data { get; set; }
        public Result(bool success, string message, T? data = default, int errorCode = 0)
        {
            Success = success;
            Message = message;
            ErrorCode = errorCode;
            Data = data;
        }


    }
    public class UserDTOS
    {
        public class LoginDTO
        {
            public LoginDTO(string userName, string password)
            {
                UserName = userName;
                Password = password;
            }

            public string UserName { get; set; }
            public string Password { get; set; }
        }
        public class UserDTO
        {
            public int UserID { get; set; }
            public int PersonID { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
            public int Role { get; set; }
            public bool IsActive { get; set; }
            [JsonConstructor]
            public UserDTO(int userID, int personID, string userName, string password, int role, bool isActive)
            {
                UserID = userID;
                PersonID = personID;
                UserName = userName;
                Password = password;
                Role = role;
                IsActive = isActive;
            }
        }
        public class UserViewDTO
        {
            public int UserID { get; set; }
            public string FullName { get; set; }
            public string UserName { get; set; }
            public string Role { get; set; }
            public bool IsActive { get; set; }
            public UserViewDTO(int userID, string FullName, string userName, string role, bool isActive)
            {
                this.UserID = userID;
                this.FullName = FullName;
                this.UserName = userName;
                this.Role = role;
                this.IsActive = isActive;
            }
        }
        public class ChangePasswordDTO
        {
            public ChangePasswordDTO(int userID, string password)
            {
                UserID = userID;
                Password = password;
            }

            public int UserID { get; set; }
            public string Password { get; set; }
        }
    }
    public class PersonDTOs
    {
        public class PersonDTO
        {
            public int PersonID { get; set; }
            public string NationalNo { get; set; }
            public string FirstName { get; set; }
            public string SecondName { get; set; }
            public string? ThirdName { get; set; }
            public string LastName { get; set; }
            public enGender Gender { get; set; }
            public string Address { get; set; }
            public string Phone { get; set; }
            public string Email { get; set; }
            public PersonDTO(int personID, string nationalNo, string firstName, string secondName,
                                string? thirdName, string lastName, enGender gender, string address, string phone, string email)
            {
                this.PersonID = personID;
                this.NationalNo = nationalNo;
                this.FirstName = firstName;
                this.SecondName = secondName;
                this.ThirdName = thirdName;
                this.LastName = lastName;
                this.Gender = gender;
                this.Address = address;
                this.Phone = phone;
                this.Email = email;
            }
        }
    }
    public class AuthorDTOs
    {
        public class AuthorDTO
        {
            public int AuthorID { get; set; }
            public int PersonID { get; set; }
            public DateTime CreatedDate { get; set; }
            [JsonConstructor]
            public AuthorDTO(int AuthorID, int PersonID, DateTime CreatedDate)
            {
                this.AuthorID = AuthorID;
                this.PersonID = PersonID;
                this.CreatedDate = CreatedDate;
            }          
        }
        public class AuthorViewDTO
        {
            public AuthorViewDTO(int authorID, string auhtorName, string email, string phone, int totalBooks)
            {
                this.authorID = authorID;
                this.authorName = auhtorName;
                this.email = email;
                this.phone = phone;
                this.totalBooks = totalBooks;
            }
            public int authorID { get; set; }
            public string authorName { get; set; }
            public string email { get; set; }
            public string phone { get; set; }
            public int totalBooks { get; set; }
        }
    }
    public class BookCategoryDTOs
    {
        public class BookCategoryDTO
        {
            public int CategoryID { get; set; }
            public string CategoryName { get; set; }

            public BookCategoryDTO(int categoryID, string categoryName)
            {
                CategoryID = categoryID;
                CategoryName = categoryName;
            }
        }
    }
    public class BookCopyDTOs
    {
        public class BookCopyDTO
        {
            public int BookCopyID { get; set; }
            public int BookID { get; set; }
            public int ReservedForMemberID { get; set; }
            public bool IsAvailabe { get; set; }
            public bool IsDamaged { get; set; }
            public BookCopyDTO(int bookCopyID, int bookID, int reservedForMemberID, bool isAvailabe, bool isDamaged)
            {
                BookCopyID = bookCopyID;
                BookID = bookID;
                ReservedForMemberID = reservedForMemberID;
                IsAvailabe = isAvailabe;
                IsDamaged = isDamaged;
            }

        }


        public class InsertCopiesDTO
        {
            public InsertCopiesDTO(int bookID, int numberOFCopies)
            {
                BookID = bookID;
                NumberOFCopies = numberOFCopies;
            }

            public int BookID { get; set; }
            public int NumberOFCopies { get; set; }
        }
        public class IsReservationAvailabeDTO
        {
            public IsReservationAvailabeDTO(int memberID, int bookID)
            {
                this.memberID = memberID;
                this.bookID = bookID;
            }

            public int memberID { get; set; }
            public int bookID { get; set; }
        }
        public class RepairCopyDTO
        {
            public RepairCopyDTO(int bookCopyID, string description, DateTime date, float cost)
            {
                this.bookCopyID = bookCopyID;
                Description = description;
                Date = date;
                Cost = cost;
            }

            public int bookCopyID { get; set; }
            public string Description { get; set; }
            public DateTime Date { get; set; }
            public float Cost { get; set; }
        }
    }
    public class PublisherDTOs
    {
        public class PublisherDTO
        {
            public int PublisherID { get; set; }
            public int PersonID { get; set; }
            public DateTime CreatedDate { get; set; }
            [JsonConstructor]
            public PublisherDTO(int publisherID, int personID, DateTime createdDate)
            {
                PublisherID = publisherID;
                PersonID = personID;
                CreatedDate = createdDate;

            }
           
        }

        public class PublisherViewDTO
        {
            public int PublisherID { get; set; }
            public string PublisherName { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public int TotalBooks { get; set; }

            public PublisherViewDTO(int publisherID, string publisherName, string email, string phone, int totalBooks)
            {
                this.PublisherID = publisherID;
                this.PublisherName = publisherName;
                this.Email = email;
                this.Phone = phone;
                this.TotalBooks = totalBooks;
            }
        }
    }
    public class BookDTOs
    {
        public class FullBookDTO
        {
            public int BookID { get; set; }
            public string Title { get; set; }
            public int AuthorID { get; set; }
            public string ISBN { get; set; }
            public int PublisherID { get; set; }
            public int CategoryID { get; set; }
            public DateTime Year { get; set; }
            public string Location { get; set; }
            public float BorrowFees { get; set; }
            public string AuthorName { get; set; }
            public string PublisherName { get; set; }
            public string CategoryName { get; set; }
            public FullBookDTO(int bookID, string title, int authorID, string isbn, int publisherID, int categoryID, DateTime year, string location, float borrowFees,
                string authorName, string publisherName, string categoryName)
            {
                BookID = bookID;
                Title = title;
                AuthorID = authorID;
                ISBN = isbn;
                PublisherID = publisherID;
                CategoryID = categoryID;
                Year = year;
                Location = location;
                BorrowFees = borrowFees;
                PublisherName = publisherName;
                AuthorName = authorName;
                CategoryName = categoryName;
            }
        }

        public class BookDTO
        {
            public BookDTO(int bookID, string title, int authorID, string iSBN, int publisherID, int categoryID, DateTime year, string location, float borrowFees)
            {
                BookID = bookID;
                Title = title;
                AuthorID = authorID;
                ISBN = iSBN;
                PublisherID = publisherID;
                CategoryID = categoryID;
                Year = year;
                Location = location;
                BorrowFees = borrowFees;
            }

            public int BookID { get; set; }
            public string Title { get; set; }
            public int AuthorID { get; set; }
            public string ISBN { get; set; }
            public int PublisherID { get; set; }
            public int CategoryID { get; set; }
            public DateTime Year { get; set; }
            public string Location { get; set; }
            public float BorrowFees { get; set; }
        }

        public class BookViewDTO
        {

            public int BookID { get; set; }
            public string Title { get; set; }
            public string Author { get; set; }
            public string ISBN { get; set; }
            public string Publisher { get; set; }
            public string Category { get; set; }
            public string Location { get; set; }
            public float BorrowFees { get; set; }
            public int TotalCopies { get; set; }
            public int AvailableCopies { get; set; }
            public BookViewDTO(int bookID, string title, string author, string iSBN, string publisher, string category, string location, float borrowFees, int totalCopies, int availableCopies)
            {
                BookID = bookID;
                Title = title;
                Author = author;
                ISBN = iSBN;
                Publisher = publisher;
                Category = category;
                Location = location;
                BorrowFees = borrowFees;
                TotalCopies = totalCopies;
                AvailableCopies = availableCopies;
            }
        }
        public class AddNewBookDTO
        {
            public AddNewBookDTO(BookDTO bookDTO, int numberOfCopies)
            {
                this.bookDTO = bookDTO;
                this.numberOfCopies = numberOfCopies;
            }

            public BookDTO bookDTO { get; set; }
            public int numberOfCopies { get; set; }
        }
    }
    public class FineDTOs
    {
        public class FineDTO
        {
            public int FineID { get; set; }
            public int MemberID { get; set; }
            public int BorrowID { get; set; }
            public float FineAmount { get; set; }
            public float PaidAmount { get; set; }
            public bool IsPaid { get; set; }
            public DateTime FineDate { get; set; }
            public int FineReason { get; set; }

            public FineDTO(int fineID, int memberID, int borrowID, float fineAmount, float paidAmount, bool isPaid, DateTime fineDate, int fineReason)
            {
                FineID = fineID;
                MemberID = memberID;
                BorrowID = borrowID;
                FineAmount = fineAmount;
                PaidAmount = paidAmount;
                IsPaid = isPaid;
                FineDate = fineDate;
                FineReason = fineReason;
            }
        }
        public class FullFineDTO
        {
            public int FineID { get; set; }
            public int MemberID { get; set; }
            public int BorrowID { get; set; }
            public float FineAmount { get; set; }
            public float PaidAmount { get; set; }
            public bool IsPaid { get; set; }
            public DateTime FineDate { get; set; }
            public string FineReason { get; set; }

            public FullFineDTO(int fineID, int memberID, int borrowID, float fineAmount, float paidAmount, bool isPaid, DateTime fineDate, string fineReason)
            {
                FineID = fineID;
                MemberID = memberID;
                BorrowID = borrowID;
                FineAmount = fineAmount;
                PaidAmount = paidAmount;
                IsPaid = isPaid;
                FineDate = fineDate;
                FineReason = fineReason;
            }
        }
        public class FineViewDTO
        {
            public int FineID { get; set; }
            public int MemberID { get; set; }
            public int BorrowID { get; set; }
            public float FineAmount { get; set; }
            public float PaidAmount { get; set; }
            public DateTime FineDate { get; set; }
            public bool IsPaid { get; set; }
            public string FineReason { get; set; }
            public FineViewDTO(int fineID, int memberID, int borrowID, float fineAmount, float paidAmount, DateTime fineDate, bool isPaid, string fineReason)
            {
                FineID = fineID;
                MemberID = memberID;
                BorrowID = borrowID;
                FineAmount = fineAmount;
                PaidAmount = paidAmount;
                FineDate = fineDate;
                IsPaid = isPaid;
                FineReason = fineReason;
            }
        }

        public class PayFineDTO
        {
            public int fineID { get; set; }
            public int paidByUserID { get; set; }
            public float paidAmount { get; set; }
            public PayFineDTO(int fineID, int paidByUserID, float paidAmount)
            {
                this.fineID = fineID;
                this.paidByUserID = paidByUserID;
                this.paidAmount = paidAmount;
            }
        }
    }
    public class MemberDTOs
    {
        public class FullMemberDTO
        {
            public int MemberID { get; set; }
            public int PersonID { get; set; }
            public DateTime CreatedDate { get; set; }
            public bool IsActive { get; set; }
            public int CreatedByUserID { get; set; }
            public MembershipDTOs.FullMembershipDTO MembershipInfoDTO { get; set; }
            public FullMemberDTO(int memberID, int personID, DateTime createdDate, bool isActive, int createdByUserID, MembershipDTOs.FullMembershipDTO membershipDTO)
            {
                MemberID = memberID;
                PersonID = personID;
                CreatedDate = createdDate;
                IsActive = isActive;
                CreatedByUserID = createdByUserID;
                MembershipInfoDTO = membershipDTO;
            }

        }

        public class RecivedDataUpdateMemberDTO
        {
            public RecivedDataUpdateMemberDTO( bool isActive )
            {
                IsActive = isActive;
            }
            public bool IsActive { get; set; }
        }
        public class ReceivedDataAddNewMemberDTO
        {
            public ReceivedDataAddNewMemberDTO(bool isActive, int createdByUserID, DateTime createdDate, MembershipDTOs.FullMembershipDTO membershipDTO)
            {
                IsActive = isActive;
                CreatedByUserID = createdByUserID;
                CreatedDate = createdDate;
                MembershipInofDTO = membershipDTO;
            }

            public bool IsActive { get; set; }
            public int CreatedByUserID { get; set; }
            public DateTime CreatedDate { get; set; }
            public MembershipDTOs.FullMembershipDTO MembershipInofDTO { get; set; }
        }
        public class MemberViewDTO
        {
            public int MemberID { get; set; }
            public string FullName { get; set; }
            public DateTime MembershipStartDate { get; set; }
            public DateTime MembershipExpirationDate { get; set; }
            public string MembershipClassName { get; set; }
            public int TotalBorrowedBook { get; set; }
            public bool IsActive { get; set; }
            public MemberViewDTO(int memberID, string fullName, DateTime membershipStartDate,
                                    DateTime membershipExpirationDate, string membershipClassName,
                                    int totalBorrowedBook, bool isActive)
            {
                this.MemberID = memberID;
                this.FullName = fullName;
                this.MembershipStartDate = membershipStartDate;
                this.MembershipExpirationDate = membershipExpirationDate;
                this.MembershipClassName = membershipClassName;
                this.TotalBorrowedBook = totalBorrowedBook;
                this.IsActive = isActive;
            }


        }
    }
    public class MembershipClassDTOs
    {
        public class MembershipClassDTO
        {
            public int MembershipClassID { get; set; }
            public string MembershipClassName { get; set; }
            public int MaxNumberOfBooksCanBorrow { get; set; }
            public float FeesPerDay { get; set; }

            public MembershipClassDTO(int membershipClassID, string membershipClassName, int maxNumberOfBooksCanBorrow, float feesPerDay)
            {
                MembershipClassID = membershipClassID;
                MembershipClassName = membershipClassName;
                MaxNumberOfBooksCanBorrow = maxNumberOfBooksCanBorrow;
                FeesPerDay = feesPerDay;
            }
        }
    }
    public class MembershipDTOs
    {
        public class MembershipDTO
        {
            public MembershipDTO(int memberID, int membershipClassID, DateTime membershipStartDate, DateTime membershipExpirationDate, float paidFees, int createdByUserID)
            {
                MemberID = memberID;
                MembershipClassID = membershipClassID;
                MembershipStartDate = membershipStartDate;
                MembershipExpirationDate = membershipExpirationDate;
                PaidFees = paidFees;
                CreatedByUserID = createdByUserID;
            }

            public int MemberID { get; set; }
            public int MembershipClassID { get; set; }
            public DateTime MembershipStartDate { get; set; }
            public DateTime MembershipExpirationDate { get; set; }
            public float PaidFees { get; set; }
            public int CreatedByUserID { get; set; }
        }
        public class FullMembershipDTO
        {
            public int MembershipID { get; set; }
            public int MemberID { get; set; }
            public int MembershipClassID { get; set; }
            public DateTime MembershipStartDate { get; set; }
            public DateTime MembershipExpirationDate { get; set; }
            public float PaidFees { get; set; }
            public int CreatedByUserID { get; set; }
            public MembershipClassDTOs.MembershipClassDTO MembershipClassInfo { get; set; }
            public string CreatedByUserName { get; set; }

            public FullMembershipDTO(int membershipID, int memberID, int membershipClassID,
                                DateTime membershipStartDate, DateTime membershipExpirationDate,
                                float paidFees, int createdByUserID, MembershipClassDTOs.MembershipClassDTO membershipClassInfo, string createdByUserName)
            {
                MembershipID = membershipID;
                MemberID = memberID;
                MembershipClassID = membershipClassID;
                MembershipStartDate = membershipStartDate;
                MembershipExpirationDate = membershipExpirationDate;
                PaidFees = paidFees;
                CreatedByUserID = createdByUserID;
                MembershipClassInfo = membershipClassInfo;
                CreatedByUserName = createdByUserName;
            }
        }
        public class RenewMembershipDTO
        {
            public RenewMembershipDTO( int membershipClassID, DateTime membershipStartDate, DateTime membershipExpirationDate, float paidFees, int createdByUserID)
            {
                MembershipClassID = membershipClassID;
                MembershipStartDate = membershipStartDate;
                MembershipExpirationDate = membershipExpirationDate;
                PaidFees = paidFees;
                CreatedByUserID = createdByUserID;
            }
            public int MembershipClassID { get; set; }
            public DateTime MembershipStartDate { get; set; }
            public DateTime MembershipExpirationDate { get; set; }
            public float PaidFees { get; set; }
            public int CreatedByUserID { get; set; }
        }
    }

    public class FineSettingsDTOs
    {
        public class FineSettingsDTO
        {
            public FineSettingsDTO(float fineAmountPerDay, float fineForDamagedBook)
            {
                FineAmountPerDay = fineAmountPerDay;
                FineForDamagedBook = fineForDamagedBook;
            }

            public float FineAmountPerDay { get; set; }
            public float FineForDamagedBook { get; set; }
        }
    }
    public class BorrowDTOs
    {
        public class FullBorrowDTO
        {
            public FullBorrowDTO(int borrowID, int memberID, int copyID, DateTime borrowDate, DateTime dueDate,
                float paidFees, string? returnNotes, DateTime? returnDate, float? returnFees,
                int createdByUserID, int? returnedByUserID, string createdByUserName,string? returnedByUserName)
            {
                BorrowID = borrowID;
                MemberID = memberID;
                CopyID = copyID;
                BorrowDate = borrowDate;
                DueDate = dueDate;
                PaidFees = paidFees;
                ReturnNotes = returnNotes;
                ReturnDate = returnDate;
                ReturnFees = returnFees;
                CreatedByUserID = createdByUserID;
                ReturnedByUserID = returnedByUserID;
                CreatedByUserName = createdByUserName;
                ReturnedByUserName = returnedByUserName;
            }

            public int BorrowID { get; set; }
            public int MemberID { get; set; }
            public int BookID { get; set; }
            public int CopyID { get; set; }
            public DateTime BorrowDate { get; set; }
            public DateTime DueDate { get; set; }
            public float PaidFees { get; set; }
            public string? ReturnNotes { get; set; }
            public DateTime? ReturnDate { get; set; }
            public float? ReturnFees { get; set; }
            public int CreatedByUserID { get; set; }
            public string CreatedByUserName { get; set; }   
            public int? ReturnedByUserID { get; set; }
            public string? ReturnedByUserName { get; set; }
        }
        public class BorrowDTO
        {
            public BorrowDTO(int borrowID, int memberID, int bookID,DateTime borrowDate, DateTime dueDate,
                float paidFees, int createdByUserID, bool hasReserrvation)
            {
                BorrowID = borrowID;
                MemberID = memberID;
                BookID = bookID;
                BorrowDate = borrowDate;
                DueDate = dueDate;
                PaidFees = paidFees;
                CreatedByUserID = createdByUserID;
                HasReserrvation = hasReserrvation;
            }

            public int BorrowID { get; set; }
            public int MemberID { get; set; }
            public int BookID { get; set; }
            public DateTime BorrowDate { get; set; }
            public DateTime DueDate { get; set; }
            public float PaidFees { get; set; }
            public int CreatedByUserID { get; set; }
            public bool HasReserrvation { get; set; } = false;
        }
        public class ReturnBookDTO
        {
            public ReturnBookDTO(int borrowID, string returnNotes, bool isDamaged, int returnedByUserID)
            {
                BorrowID = borrowID;
                ReturnNotes = returnNotes;
                IsDamaged = isDamaged;
                ReturnedByUserID = returnedByUserID;
            }
            public int BorrowID { get; set; }
            public string ReturnNotes { get; set; }
            public bool IsDamaged { get; set; }
            public int ReturnedByUserID { get; set; }
        }
        public class ReturnedBookDTO
        {
            public ReturnedBookDTO(float returnFees, int reservedForMemberID)
            {
                ReturnFees = returnFees;
                ReservedForMemberID = reservedForMemberID;
            }
            public float ReturnFees { get; set; }
            public int ReservedForMemberID { get; set; }
        }
        public class BorrowViewDTO
        {
            public int BorrowID { get; set; }
            public int MemberID { get; set; }
            public string MemberName { get; set; }
            public string Title { get; set; }
            public int BookCopyID { get; set; }
            public bool IsReturned { get; set; }
            public BorrowViewDTO(int BorrowID, int MemberID, string MemberName, string Title, int BookCopyID, bool IsReturned)
            {
                this.BorrowID = BorrowID;
                this.MemberID = MemberID;
                this.MemberName = MemberName;
                this.Title = Title;
                this.BookCopyID = BookCopyID;
                this.IsReturned = IsReturned;
            }
        }
        public class BorrowedBookDTO
        {
            public BorrowedBookDTO(int borrowID, int copyID)
            {
                BorrowID = borrowID;
                CopyID = copyID;
            }
            public int BorrowID { get; set; }
            public int CopyID { get; set; }

        }
    }
    public class ReservationDTOs
    {
        public class HasActiveReservationDTO
        {
            public int memberID;
            public int bookID;

            public HasActiveReservationDTO(int memberID, int bookID)
            {
                this.memberID = memberID;
                this.bookID = bookID;
            }
        }
        public class GetReservationIDDTO
        {
            public int BookID;
            public int MemberID;
            public GetReservationIDDTO(int bookID, int memberID)
            {
                BookID = bookID;
                MemberID = memberID;
            }
        }
        public class AddNewReservationDTO
        {
            public int MemberID { get; set; }
            public int BookID { get; set; }
            public int CreatedByUserID { get; set; }
            [JsonConstructor]
            public AddNewReservationDTO(int memberID, int bookID, int createdByUserID)
            {
                MemberID = memberID;
                BookID = bookID;
                CreatedByUserID = createdByUserID;
            }
        }
        public class ReservationDTO
        {
            public ReservationDTO(int reservationID, int memberID, int bookID, DateTime reservationDate,
                enReservationStatus reservationStatus, int createdByUserID)
            {
                ReservationID = reservationID;
                MemberID = memberID;
                BookID = bookID;
                ReservationDate = reservationDate;
                ReservationStatus = reservationStatus;
                CreatedByUserID = createdByUserID;
            }

            public int ReservationID { get; set; }
            public int MemberID { get; set; }
            public int BookID { get; set; }
            public DateTime ReservationDate { get; set; }
            public enReservationStatus ReservationStatus { get; set; }
            public int CreatedByUserID { get; set; }
        }
        public class ReservationViewDTO
        {
            public ReservationViewDTO(int reservationID, int memberID, int bookID, DateTime reservationDate, string reservationStatus)
            {
                ReservationID = reservationID;
                MemberID = memberID;
                BookID = bookID;
                ReservationDate = reservationDate;
                ReservationStatus = reservationStatus;
            }

            public int ReservationID { get; set; }
            public int MemberID { get; set; }
            public int BookID { get; set; }
            public DateTime ReservationDate { get; set; }
            public string ReservationStatus { get; set; }
        }
    }
}



