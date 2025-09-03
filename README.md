# FOOD SYNC(ASP.NET MVC)

## Overview
This is a complete food delivery web application developed in ASP.NET MVC with Entity Framework, designed to streamline both customer ordering and business management processes.

### Key Features & Functionalities

**Customer Management**
- Secure registration & login system
- Personalized user dashboard
- Order history tracking (Pending → Processing → Shipped status tracking)

**Product Management**
- Product catalog with categories, descriptions, and pricing
- Search & filter functionality
- Dynamic image management for products

**Shopping Experience**
- Multi-step checkout process
- Cart management (Add/Remove/Edit quantities)
- Guest checkout option

**Order Management**
- Admin order status updates
- Invoice generation
- Stock management integrated

**Content Management**
- Blog posts for marketing and news
- Contact form system
- Message management dashboard

**User Interface**
- Responsive design (Bootstrap 5)
- Interactive UI elements (jQuery plugins)
- Admin dashboard with MDI layout

## Technologies

### Core Stack
- **Languages**: C#, HTML5, CSS3, JavaScript
- **Framework**: ASP.NET MVC 5
- **Database**: Microsoft SQL Server (MDF/LDF files included)
- **ORM**: Entity Framework 6.5.1

### Frontend Libraries
- Bootstrap 5.3.7
- jQuery 3.7.1
- jQuery Validation
- Modernizr

### Development Tools
- Visual Studio 2022 (via .sln file)
- Code First Migrations (Entity Framework)
- HTML Helpers & Partial Views
- ViewModel pattern implementation

## System Architecture

### Database Schema (ERD)
![](S13Screen.PNG)

**Main Entities**:
- Users, Products, Orders, Carts
- Admin Accounts, Blogs, Messages
- Invoices with order linking

### Use Cases
![](S5Screen.PNG) | ![](S3Screen.PNG)

**User Roles**:
- **Customers**: Place orders, track shipments, contact support
- **Admins**: Manage products, update order status, generate invoices, manage blogs
- **Guest Users**: Browse catalog, submit contact forms

## Getting Started

### Setup Instructions
1. Clone repository
2. Open with Visual Studio 2022
3. Restore NuGet packages (Tools → NuGet Package Manager)
4. Update database with migrations (`Update-Database`)
5. Run application (F5)

### File Structure
```
/Controllers         → Business logic and routing
/Models              → Database models and data layer
/Views               → HTML templates and layout pages
/Content/assets      → CSS, images, fonts
/Scripts             → JavaScript and plugin files
/Migrations          → Database schema versioning
```

## Screenshots

### User Interface
![](S8Screen.PNG)  
*Customer Dashboard with Product Catalog*

### Admin Panel
![](S3Screen.PNG)  
*Order Management Interface*

![](S5Screen.PNG)  
*Product Catalog Management Dashboard*


## Contributing
Contributions are welcome! Fork the project, create a feature branch, and submit a pull request.

## License
This project is MIT licensed – see the LICENSE.md file for details.

## Contact
For issues or queries, contact the maintainers via GitHub project page.
