using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BLL.Services.Admin.Interfaces;
using BLL.Services.Util.Interfaces;
using DAL.DTOs.BookDTO;
using DAL.DTOs.CategoryDTOs;
using DAL.Mapper;
using DAL.Models;
using DAL.Repositories.Admin;
using DAL.Repositories.Admin.Interfaces;

namespace BLL.Services.Admin
{
    public class BookService : IBookService
    {
        private readonly ICategoryRepository _categoryRepository;

        private readonly IBookRepository _bookRepository;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IAuthorRepository _iAuthorRepository;
        private readonly IMapper _mapper;
        

        public BookService(IBookRepository bookRepository, ICloudinaryService cloudinaryService, IMapper mapper, IAuthorRepository iAuthorRepository, ICategoryRepository categoryRepository)
        {
            _bookRepository = bookRepository;
            _cloudinaryService = cloudinaryService;
            _mapper = mapper;
            _iAuthorRepository = iAuthorRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<List<BookResponeDTO>> GetAllAsync()
        {
           var books= (await _bookRepository.GetAllAsync()).ToList();
            List<BookResponeDTO> listDTO = _mapper.Map<List<BookResponeDTO>>(books);
            for (int i = 0; i < listDTO.Count(); i++)
            { 
                var bookcate = _mapper.Map<List<CategoryResponseDto>>(books[i].Categories);
                listDTO[i].Categories= bookcate;
                listDTO[i].AuthorName = books[i].Author?.AuthorName;
            }
            return listDTO;
        }

        public async Task<List<BookResponeDTO>> SearchAsync(string? search, List<int>? categoryIds)
        {
            var books = (await _bookRepository.SearchAsync(search, categoryIds)).ToList();
            List<BookResponeDTO> listDTO = _mapper.Map<List<BookResponeDTO>>(books);
            for (int i = 0; i < listDTO.Count(); i++)
            {
                listDTO[i].Categories = _mapper.Map<List<CategoryResponseDto>>(books[i].Categories);
                listDTO[i].AuthorName = books[i].Author?.AuthorName;
            }
            return listDTO;
        }

        public async Task<BookResponeDTO?> GetByIdAsync(int id)
        {
            BookResponeDTO dtoRespone = new BookResponeDTO();
            var book = await _bookRepository.GetByIdAsync(id);
            if (book != null)
            {
                dtoRespone = _mapper.Map<BookResponeDTO>(book);
                dtoRespone.AuthorName = book.Author?.AuthorName;
                dtoRespone.Categories = (List<CategoryResponseDto>)_mapper.Map<IEnumerable<CategoryResponseDto>>(book.Categories);
            }
            
            return dtoRespone;
        }

        public async Task<BookResponeDTO> CreateAsync(BookCreateDTO dto)
        {
            var imageUrl = await _cloudinaryService.UploadImageAsync(dto.CoverImageUrl);

            var book = _mapper.Map<Book>(dto);
            book.CoverImageUrl = imageUrl;
            
            // Handle Author logic
            var existingAuthor = await _iAuthorRepository.GetByNameAsync(dto.AuthorName);
             if (existingAuthor != null)
            {
                book.AuthorId = existingAuthor.AuthorId;
            }
            else
            {
                book.Author = new Author { AuthorName = dto.AuthorName };
            }

            // Handle Categories logic
             if (dto.CategoryIds != null && dto.CategoryIds.Any())
            {
                List<Category> listCate = await _categoryRepository.GetListByIdAsync(dto.CategoryIds);
                book.Categories = listCate;
            }

            // Save to Database
            await _bookRepository.CreateAsync(book);

            // Mapping AFTER insertion to ensure BookId is populated
            var dtoRespone = _mapper.Map<BookResponeDTO>(book);
            
            // Populate AuthorName and Categories for the response
            dtoRespone.AuthorName = book.Author?.AuthorName ?? dto.AuthorName;
            dtoRespone.Categories = _mapper.Map<List<CategoryResponseDto>>(book.Categories);

            return dtoRespone;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _bookRepository.DeleteAsync(id);
        }

        public async Task<BookResponeDTO?> UpdateAsync(int id, BookUpdateDTO dto)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null) return null;

            // Update basic info
            book.Title = dto.Title;
            book.Price = dto.Price;
            book.ISBN = dto.ISBN;
            book.Publisher = dto.Publisher;
            book.PublishDate = dto.PublishDate;
            book.Description = dto.Description;
            book.StockQuantity = dto.StockQuantity;

            // Update image if provided
            if (dto.CoverImageUrl != null)
            {
                var imageUrl = await _cloudinaryService.UploadImageAsync(dto.CoverImageUrl);
                book.CoverImageUrl = imageUrl;
            }

            // Handle Author
            if (string.IsNullOrEmpty(dto.AuthorName))
            {
                book.AuthorId = null;
                book.Author = null;
            }
            else if (book.Author == null || !string.Equals(book.Author.AuthorName, dto.AuthorName, StringComparison.OrdinalIgnoreCase))
            {
                var existingAuthor = await _iAuthorRepository.GetByNameAsync(dto.AuthorName);
                if (existingAuthor != null)
                {
                    book.AuthorId = existingAuthor.AuthorId;
                    book.Author = existingAuthor; // Set the object to let EF know the relationship
                }
                else
                {
                    book.Author = new Author { AuthorName = dto.AuthorName };
                    book.AuthorId = null;
                }
            }

            // Handle Categories (Many-to-Many)
            if (dto.CategoryIds != null)
            {
                book.Categories.Clear();
                var newCategories = await _categoryRepository.GetListByIdAsync(dto.CategoryIds);
                foreach (var category in newCategories)
                {
                    book.Categories.Add(category);
                }
            }

            var success = await _bookRepository.UpdateAsync(book);
            if (!success) return null;

            // Prepare response
            var response = _mapper.Map<BookResponeDTO>(book);
            var updatedBook = await _bookRepository.GetByIdAsync(id); // Reload to get navigation properties correctly
            if (updatedBook != null)
            {
                response.AuthorName = updatedBook.Author?.AuthorName;
                response.Categories = _mapper.Map<List<CategoryResponseDto>>(updatedBook.Categories);
            }

            return response;
        }
    }
}
