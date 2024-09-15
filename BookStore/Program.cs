using BookStore.Data;
using BookStore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookStore
{
    class Program
    {
        static void Main()
        {
            using (var context = new ApplicationContextFactory().CreateDbContext())
            {
                while (true)
                {
                    Console.WriteLine("Выберите действие:");
                    Console.WriteLine("1. Управление книгами");
                    Console.WriteLine("2. Управление категориями");
                    Console.WriteLine("3. Управление авторами");
                    Console.WriteLine("4. Управление заказами");
                    Console.WriteLine("0. Выход");

                    var choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            ManageBooks(context);
                            break;
                        case "2":
                            ManageCategories(context);
                            break;
                        case "3":
                            ManageAuthors(context);
                            break;
                        case "4":
                            ManageOrders(context);
                            break;
                        case "0":
                            return;
                        default:
                            Console.WriteLine("Неверный выбор.");
                            break;
                    }
                }
            }
        }

        #region Книги
        static void ManageBooks(ApplicationContext context)
        {
            Console.WriteLine("Выберите действие для книг:");
            Console.WriteLine("1. Добавить книгу");
            Console.WriteLine("2. Удалить книгу");
            Console.WriteLine("3. Редактировать книгу");
            Console.WriteLine("4. Найти книгу");
            Console.WriteLine("5. Отобразить все книги");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    AddBook(context);
                    break;
                case "2":
                    DeleteBook(context);
                    break;
                case "3":
                    EditBook(context);
                    break;
                case "4":
                    SearchBook(context);
                    break;
                case "5":
                    DisplayBooks(context);
                    break;
                default:
                    Console.WriteLine("Неверный выбор.");
                    break;
            }
        }

        static void AddBook(ApplicationContext context)
        {
            Console.WriteLine("Введите название книги:");
            string? title = Console.ReadLine();
            Console.WriteLine("Введите описание книги:");
            string? description = Console.ReadLine();
            Console.WriteLine("Введите цену:");
            decimal price = decimal.Parse(Console.ReadLine());

            Console.WriteLine("Введите ID категории книги:");
            int categoryId = int.Parse(Console.ReadLine());

            Console.WriteLine("Введите IDs авторов книги, разделенные запятой:");
            string? authorIdsInput = Console.ReadLine();
            var authorIds = authorIdsInput?.Split(',').Select(id => int.Parse(id.Trim())).ToList();

            var category = context.Categories.Find(categoryId);
            var authors = context.Authors.Where(a => authorIds.Contains(a.Id)).ToList();

            if (category != null && authors.Any())
            {
                var book = new Book
                {
                    Title = title,
                    Description = description,
                    Price = price,
                    PublishedOn = DateTime.Now,
                    CategoryId = categoryId,
                    Authors = authors
                };

                context.Books.Add(book);
                context.SaveChanges();
                Console.WriteLine("Книга добавлена.");
            }
            else
            {
                if (category == null)
                {
                    Console.WriteLine("Категория не найдена.");
                }
                if (!authors.Any())
                {
                    Console.WriteLine("Авторы не найдены.");
                }
            }
        }

        static void DeleteBook(ApplicationContext context)
        {
            Console.WriteLine("Введите ID книги для удаления:");
            int id = int.Parse(Console.ReadLine());

            var book = context.Books.Include(b => b.Authors).Include(b => b.Category).FirstOrDefault(b => b.Id == id);
            if (book != null)
            {
                context.Books.Remove(book);
                context.SaveChanges();
                Console.WriteLine("Книга удалена.");
            }
            else
            {
                Console.WriteLine("Книга не найдена.");
            }
        }

        static void EditBook(ApplicationContext context)
        {
            Console.WriteLine("Введите ID книги для редактирования:");
            int id = int.Parse(Console.ReadLine());

            var book = context.Books.Include(b => b.Authors).Include(b => b.Category).FirstOrDefault(b => b.Id == id);
            if (book != null)
            {
                Console.WriteLine("Введите новое название книги (или Enter для пропуска):");
                string title = Console.ReadLine();
                if (!string.IsNullOrEmpty(title))
                {
                    book.Title = title;
                }

                Console.WriteLine("Введите новое описание книги (или Enter для пропуска):");
                string description = Console.ReadLine();
                if (!string.IsNullOrEmpty(description))
                {
                    book.Description = description;
                }

                Console.WriteLine("Введите новую цену книги (или Enter для пропуска):");
                string priceInput = Console.ReadLine();
                if (decimal.TryParse(priceInput, out decimal price))
                {
                    book.Price = price;
                }

                Console.WriteLine("Введите новый ID категории книги (или Enter для пропуска):");
                string categoryIdInput = Console.ReadLine();
                if (int.TryParse(categoryIdInput, out int categoryId))
                {
                    var category = context.Categories.Find(categoryId);
                    if (category != null)
                    {
                        book.CategoryId = categoryId;
                        book.Category = category;
                    }
                }

                Console.WriteLine("Введите новые IDs авторов книги, разделенные запятой (или Enter для пропуска):");
                string authorIdsInput = Console.ReadLine();
                if (!string.IsNullOrEmpty(authorIdsInput))
                {
                    var authorIds = authorIdsInput.Split(',').Select(id => int.Parse(id.Trim())).ToList();
                    var authors = context.Authors.Where(a => authorIds.Contains(a.Id)).ToList();
                    if (authors.Any())
                    {
                        book.Authors = authors;
                    }
                }

                context.SaveChanges();
                Console.WriteLine("Книга обновлена.");
            }
            else
            {
                Console.WriteLine("Книга не найдена.");
            }
        }

        static void SearchBook(ApplicationContext context)
        {
            Console.WriteLine("Введите название книги для поиска:");
            string title = Console.ReadLine();

            var books = context.Books
                .Include(b => b.Authors)
                .Include(b => b.Category)
                .Where(b => b.Title.Contains(title))
                .ToList();

            foreach (var book in books)
            {
                Console.WriteLine($"Книга: {book.Title}, Категория: {book.Category?.Name}, Авторы: {string.Join(", ", book.Authors.Select(a => a.Name))}, Цена: {book.Price}, Описание: {book.Description}");
            }
        }

        static void DisplayBooks(ApplicationContext context)
        {
            var books = context.Books
                .Include(b => b.Authors)
                .Include(b => b.Category)
                .ToList();

            foreach (var book in books)
            {
                Console.WriteLine($"Книга: {book.Title}, Категория: {book.Category?.Name}, Авторы: {string.Join(", ", book.Authors.Select(a => a.Name))}, Цена: {book.Price}, Описание: {book.Description}");
            }
        }
        #endregion

        #region Категории
        static void ManageCategories(ApplicationContext context)
        {
            Console.WriteLine("Выберите действие для категорий:");
            Console.WriteLine("1. Добавить категорию");
            Console.WriteLine("2. Удалить категорию");
            Console.WriteLine("3. Редактировать категорию");
            Console.WriteLine("4. Найти категорию");
            Console.WriteLine("5. Отобразить все категории");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    AddCategory(context);
                    break;
                case "2":
                    DeleteCategory(context);
                    break;
                case "3":
                    EditCategory(context);
                    break;
                case "4":
                    SearchCategory(context);
                    break;
                case "5":
                    DisplayCategories(context);
                    break;
                default:
                    Console.WriteLine("Неверный выбор.");
                    break;
            }
        }

        static void AddCategory(ApplicationContext context)
        {
            Console.WriteLine("Введите название категории:");
            string? name = Console.ReadLine();

            var category = new Category { Name = name };
            context.Categories.Add(category);
            context.SaveChanges();
            Console.WriteLine("Категория добавлена.");
        }

        static void DeleteCategory(ApplicationContext context)
        {
            Console.WriteLine("Введите ID категории для удаления:");
            int id = int.Parse(Console.ReadLine());

            var category = context.Categories.Find(id);
            if (category != null)
            {
                context.Categories.Remove(category);
                context.SaveChanges();
                Console.WriteLine("Категория удалена.");
            }
            else
            {
                Console.WriteLine("Категория не найдена.");
            }
        }

        static void EditCategory(ApplicationContext context)
        {
            Console.WriteLine("Введите ID категории для редактирования:");
            int id = int.Parse(Console.ReadLine());

            var category = context.Categories.Find(id);
            if (category != null)
            {
                Console.WriteLine("Введите новое название категории (или Enter для пропуска):");
                string name = Console.ReadLine();
                if (!string.IsNullOrEmpty(name))
                {
                    category.Name = name;
                }

                context.SaveChanges();
                Console.WriteLine("Категория обновлена.");
            }
            else
            {
                Console.WriteLine("Категория не найдена.");
            }
        }

        static void SearchCategory(ApplicationContext context)
        {
            Console.WriteLine("Введите название категории для поиска:");
            string name = Console.ReadLine();

            var categories = context.Categories
                .Where(c => c.Name.Contains(name))
                .ToList();

            foreach (var category in categories)
            {
                Console.WriteLine($"Категория: {category.Name}");
            }
        }

        static void DisplayCategories(ApplicationContext context)
        {
            var categories = context.Categories.ToList();
            foreach (var category in categories)
            {
                Console.WriteLine($"Категория: {category.Name}");
            }
        }
        #endregion

        #region Авторы
        static void ManageAuthors(ApplicationContext context)
        {
            Console.WriteLine("Выберите действие для авторов:");
            Console.WriteLine("1. Добавить автора");
            Console.WriteLine("2. Удалить автора");
            Console.WriteLine("3. Редактировать автора");
            Console.WriteLine("4. Найти автора");
            Console.WriteLine("5. Отобразить всех авторов");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    AddAuthor(context);
                    break;
                case "2":
                    DeleteAuthor(context);
                    break;
                case "3":
                    EditAuthor(context);
                    break;
                case "4":
                    SearchAuthor(context);
                    break;
                case "5":
                    DisplayAuthors(context);
                    break;
                default:
                    Console.WriteLine("Неверный выбор.");
                    break;
            }
        }

        static void AddAuthor(ApplicationContext context)
        {
            Console.WriteLine("Введите имя автора:");
            string? name = Console.ReadLine();

            var author = new Author { Name = name };
            context.Authors.Add(author);
            context.SaveChanges();
            Console.WriteLine("Автор добавлен.");
        }

        static void DeleteAuthor(ApplicationContext context)
        {
            Console.WriteLine("Введите ID автора для удаления:");
            int id = int.Parse(Console.ReadLine());

            var author = context.Authors.Find(id);
            if (author != null)
            {
                context.Authors.Remove(author);
                context.SaveChanges();
                Console.WriteLine("Автор удален.");
            }
            else
            {
                Console.WriteLine("Автор не найден.");
            }
        }

        static void EditAuthor(ApplicationContext context)
        {
            Console.WriteLine("Введите ID автора для редактирования:");
            int id = int.Parse(Console.ReadLine());

            var author = context.Authors.Find(id);
            if (author != null)
            {
                Console.WriteLine("Введите новое имя автора (или Enter для пропуска):");
                string name = Console.ReadLine();
                if (!string.IsNullOrEmpty(name))
                {
                    author.Name = name;
                }

                context.SaveChanges();
                Console.WriteLine("Автор обновлен.");
            }
            else
            {
                Console.WriteLine("Автор не найден.");
            }
        }

        static void SearchAuthor(ApplicationContext context)
        {
            Console.WriteLine("Введите имя автора для поиска:");
            string name = Console.ReadLine();

            var authors = context.Authors
                .Where(a => a.Name.Contains(name))
                .Include(a => a.Books)
                .ToList();

            foreach (var author in authors)
            {
                var bookTitles = string.Join(", ", author.Books.Select(b => b.Title));
                Console.WriteLine($"Автор: {author.Name}, Книги: {bookTitles}");
            }
        }

        static void DisplayAuthors(ApplicationContext context)
        {
            var authors = context.Authors
                .Include(a => a.Books)
                .ToList();
            foreach (var author in authors)
            {
                var bookTitles = string.Join(", ", author.Books.Select(b => b.Title));
                Console.WriteLine($"Автор: {author.Name}, Книги: {bookTitles}");
            }
        }
        #endregion

        #region Заказы
        static void ManageOrders(ApplicationContext context)
        {
            Console.WriteLine("Выберите действие для заказов:");
            Console.WriteLine("1. Добавить заказ");
            Console.WriteLine("2. Удалить заказ");
            Console.WriteLine("3. Редактировать заказ");
            Console.WriteLine("4. Найти заказ");
            Console.WriteLine("5. Отобразить все заказы");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    AddOrder(context);
                    break;
                case "2":
                    DeleteOrder(context);
                    break;
                case "3":
                    EditOrder(context);
                    break;
                case "4":
                    SearchOrder(context);
                    break;
                case "5":
                    DisplayOrders(context);
                    break;
                default:
                    Console.WriteLine("Неверный выбор.");
                    break;
            }
        }

        static void AddOrder(ApplicationContext context)
        {
            Console.WriteLine("Введите ID книги:");
            int bookId = int.Parse(Console.ReadLine());

            Console.WriteLine("Введите количество:");
            int quantity = int.Parse(Console.ReadLine());

            var book = context.Books.Find(bookId);
            if (book != null)
            {
                var order = new Order
                {
                    BookId = bookId,
                    Quantity = quantity,
                    OrderDate = DateTime.Now,
                    Book = book
                };

                context.Orders.Add(order);
                context.SaveChanges();
                Console.WriteLine("Заказ добавлен.");
            }
            else
            {
                Console.WriteLine("Книга не найдена.");
            }
        }

        static void DeleteOrder(ApplicationContext context)
        {
            Console.WriteLine("Введите ID заказа для удаления:");
            int id = int.Parse(Console.ReadLine());

            var order = context.Orders.Find(id);
            if (order != null)
            {
                context.Orders.Remove(order);
                context.SaveChanges();
                Console.WriteLine("Заказ удален.");
            }
            else
            {
                Console.WriteLine("Заказ не найден.");
            }
        }

        static void EditOrder(ApplicationContext context)
        {
            Console.WriteLine("Введите ID заказа для редактирования:");
            int id = int.Parse(Console.ReadLine());

            var order = context.Orders.Include(o => o.Book).FirstOrDefault(o => o.Id == id);
            if (order != null)
            {
                Console.WriteLine("Введите новое количество (или Enter для пропуска):");
                string quantityInput = Console.ReadLine();
                if (int.TryParse(quantityInput, out int quantity))
                {
                    order.Quantity = quantity;
                }

                context.SaveChanges();
                Console.WriteLine("Заказ обновлен.");
            }
            else
            {
                Console.WriteLine("Заказ не найден.");
            }
        }

        static void SearchOrder(ApplicationContext context)
        {
            Console.WriteLine("Введите ID книги для поиска заказов:");
            int bookId = int.Parse(Console.ReadLine());

            var orders = context.Orders
                .Include(o => o.Book)
                .Where(o => o.BookId == bookId)
                .ToList();

            foreach (var order in orders)
            {
                Console.WriteLine($"Заказ: Книга: {order.Book.Title}, Количество: {order.Quantity}, Дата заказа: {order.OrderDate}");
            }
        }

        static void DisplayOrders(ApplicationContext context)
        {
            var orders = context.Orders.Include(o => o.Book).ToList();
            foreach (var order in orders)
            {
                Console.WriteLine($"Заказ: Книга: {order.Book.Title}, Количество: {order.Quantity}, Дата заказа: {order.OrderDate}");
            }
        }
        #endregion
    }
}
