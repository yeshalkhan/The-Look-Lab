# The Look Lab
The Look Lab is a user-friendly cosmetics website designed using HTML, CSS, Bootstrap and ASP.Net, allowing users to shop for makeup products at ease.

## Prominent Features:
- Elegant and Responsive Design: Trendy, user-friendly HTML pages designed for easy navigation and a visually pleasing experience.
- Inventory Management: Allows admin to add, update, delete products.
-	Secure Shopping Cart: Customers can add, remove and view items in their shopping cart anytime, anywhere.
- Admin Dashboard: Secure admin panel to allow admin to view website analytics, products, users and orders
- User Authentication: Uses ASP.Net Core Identity to manage users. Users can login or register smoothly.
- User Authorization: Implements claim-based authorization to restrict access, ensuring only administrators can access the admin dashboard.
- Order Management: Allows customers to place orders and admin to view each order's details and products.
- Session Management: Uses sessions to store unregistered users' cart data.
- Database Management: Uses Generic Repsoitory pattern to store infromation about users, products, cart, orders, order details in database.

## Screenshots:
Find screenshots of the website in the attached document: [Preview.pdf](https://github.com/user-attachments/files/17118747/Preview.pdf)

## Tech Stack:
- HTML/CSS to design front end
- Bootstrap for responsiveness
- Minor Javascript for front-end functionalities
- ASP.Net Core MVC framework
- ASP.NET Core API framework

## This project uses:
- Clean Architecture
- Dapper, Generic Repository pattern, Repository Decorator pattern for Database management
- Dependency Injection pattern for loose coupling
- ASP.Net Core Identity for User management in MVC
- JWT Authentication for User management in API
- Asynchronous programming and Caching to enhance speed
- SignalR for real time stock updates
- AJAX for partially refreshing pages
