using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokPractice.DAL;
using PustokPractice.Extensions;
using PustokPractice.Models;

namespace PustokPractice.Areas.Manage.Controllers
{
    [Area("manage")]


    public class BookController : Controller
    {
        private readonly AppDbContext _context;

        public BookController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var books = _context.Books.ToList();
            return View(books);
        }
        public IActionResult Create()
        {
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genre = _context.Genre.ToList();
            ViewBag.Tags = _context.Tags.ToList();
            return View();
        }
        [HttpPost]
        public IActionResult Create(Book book)
        {
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genre = _context.Genre.ToList();
            ViewBag.Tags = _context.Tags.ToList();

            if (!ModelState.IsValid)
            {

                return View();
            }
            if (!_context.Genre.Any(a => a.id == book.GenreId))
            {
                ModelState.AddModelError("GenreId", "genre is not found!");
                return View();
            }

            if (!_context.Authors.Any(a => a.id == book.AuthorId))
            {
                ModelState.AddModelError("AuthorId", "Author is not found!");
                return View();
            }
            bool check = true;
            if (book.TagIds != null)
            {
                foreach (var item in book.TagIds)
                {
                    if (!_context.Tags.Any(x => x.Id == item))
                    {
                        check = false;
                        break;
                    }
                }
            }
            if (check)
            {
                foreach (var item in book.TagIds)
                {
                    BookTag bookTag = new BookTag()
                    {
                        Book = book,
                        TagId = item,
                    };
                    _context.BookTags.Add(bookTag);
                }
            }
            else
            {
                ModelState.AddModelError("TagId", "Error");
                return View();
            }

            _context.Books.Add(book);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Update(int id)
        {
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genre = _context.Genre.ToList();
            ViewBag.Tags = _context.Tags.ToList();

            Book existBook = _context.Books.Include(x => x.BookTags).FirstOrDefault(x => x.id == id);
            if (existBook == null) return NotFound();

            existBook.TagIds = existBook.BookTags.Select(x => x.TagId).ToList();
            return View(existBook);


        }
        [HttpPost]
        public IActionResult Update(Book book)
        {
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genre = _context.Genre.ToList();
            ViewBag.Tags = _context.Tags.ToList();

            if (!ModelState.IsValid)
            {
                return View();
            }
            Book existBook = _context.Books.Include(x => x.BookTags).FirstOrDefault(x => x.id == book.id);
            if (existBook == null) return NotFound();


            existBook.BookTags.RemoveAll(x => !book.TagIds.Any(y => y == x.TagId));

            foreach (var item in book.TagIds.Where(x => !existBook.BookTags.Any(y => y.TagId == x)))
            {
                BookTag bookTag = new BookTag()
                {
                    TagId = item,
                };
                _context.BookTags.Add(bookTag);
            }

            existBook.Name = book.Name;
            existBook.Description = book.Description;
            existBook.Costprice = book.Costprice;
            existBook.Saleprice = book.Saleprice;
            existBook.DiscountPercent = book.DiscountPercent;
            existBook.Code = book.Code;
            existBook.IsAvailable = book.IsAvailable;
            existBook.Tax = book.Tax;
            existBook.GenreId = book.GenreId;
            existBook.AuthorId = book.AuthorId;

            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
