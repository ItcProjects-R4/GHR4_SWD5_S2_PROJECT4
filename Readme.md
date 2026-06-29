# 🎓 LMSify - E-Learning Platform (LMS)

A production-ready, secure, and modern private e-learning academy platform built with **ASP.NET Core 8.0 MVC (C#)**. It is designed for independent educators and academies to host, showcase, and sell their courses in a unified portal without third-party portal dependencies.

---

## 📖 Project Concept
**LMSify** is a centralized academy platform for a single educational academy or solo instructor (rather than a multi-vendor marketplace like Udemy). 

The platform integrates standard learning management features directly with a **unified course player workspace**. Instead of redirecting students between disjointed tabs, all lectures, articles, assessments, and progress metrics are loaded dynamically via AJAX, keeping the student fully engaged in a single workspace.

---

## 🎯 Objectives
*   **Centralized content delivery:** Provide one home for streaming lectures, reading articles, and downloading resources.
*   **Onboarding Automation:** Eliminate the administrative bottleneck of manually checking payments and receipts to enroll students.
*   **Instructor Delegation:** Allow the primary instructor to delegate grading tasks to Teaching Assistants with customized access permissions.
*   **Asynchronous UX:** Offer a fluid, fast-loading, single-page application experience for the course study panel.
*   **Enterprise Security:** Implement robust authorization boundaries to protect intellectual property and user databases.

---

## ⚡ Tech Stack & Core Integrations
*   **Backend Framework:** ASP.NET Core 10.0 MVC (C#)
*   **Database & ORM:** SQL Server via Entity Framework Core (Code-First)
*   **Identity & Access Control:** ASP.NET Identity Core (Role-Based Authorization claims)
*   **Payment Processing:** Paymob Integration (HMAC-SHA512 verification webhooks)
*   **Media & Asset Hosting:** Cloudinary SDK (Raw files & video streams)
*   **Mailing System:** MailKit / SMTP Integration

---

## 🔐 Key Features

### 1. Interactive Course Player (Student Workspace)
*   **Asynchronous Content Rendering:** Uses AJAX to dynamically load content (videos, HTML articles, assignments) inside a single page, eliminating full-page refresh delays.
*   **Multi-Format Learning Material:** Supports Cloudinary-hosted MP4 streaming videos, rich-text custom articles, and external resource links.
*   **Homework Assignment Widget:** Allows students to upload files or submit GitHub links directly inside the course player view.
### 2. Automated Paymob Checkout & Onboarding
*   **Secure Payment Integration:** Integrates Paymob Accept for online card payments with HMAC-SHA512 webhook signature verification.
*   **Zero-Touch Provisioning:** The webhook listener instantly creates an active student enrollment in the database once payment is successful.
*   **Free Course Instant Checkout:** Free courses bypass checkout payment frames and enroll the student with a single click.
### 3. Curriculum Builder (Instructor Workspace)
*   **Multi-Step Syllabus Wizard:** Step 1 captures course metadata, descriptions, prices, and uploads Cloudinary thumbnails. Step 2 allows adding modules, contents, and assignments.
*   **Article Editor:** A full-screen WYSIWYG text editor allowing instructors to format headers, bold/italic text, lists, and links, saving HTML content directly into the database.
### 4. Staff Delegation & Access Control
*   **Teaching Assistant (TA) Creation:** The main instructor can register TAs directly from their dashboard.
*   **Custom Role Claims:** Custom permission claims are assigned to control what assistants can do (e.g. grading vs. course editing).
*   **Secure Data Isolation (BOLA Protection):** Server-side verification filters ensure instructors and assistants can only view or edit submissions for courses they actually own.
### 5. Assignment Grading System
*   **Submission Tracking:** A dashboard listing pending and graded assignments uploaded by students.
*   **Interactive Gradebooks:** Assistants can review student files, assign numeric grades, and leave detailed feedback comment cards.
### 6. Public Portal & Discovery
*   **Dynamic Catalog:** Browse all courses with search, sorting (price, oldest/newest, alphabetical), and live enrollment counts.
*   **Duplicate Safeguards:** Validates existing database enrollments or newsletter subscriptions to prevent double records.
---

## 🚀 Getting Started & Installation

### Prerequisites
*   Visual Studio 2026 / JetBrains Rider
*   .NET SDK 10.0
*   Local SQL Server Express or LocalDB

### Local Setup
1.  **Clone the repository:**
    ```bash
    git clone https://github.com/Mostafa-Y3sser/LMS.git
    ```
2.  **Update Database Connection Strings:**
    Configure `appsettings.json` inside the `LMS.PL` directory with your local database server details.
3.  **Run migrations & update database:**
    ```bash
    dotnet ef database update --project LMS.DAL --startup-project LMS.PL
    ```
4.  **Launch the project:**
    ```bash
    dotnet run --project LMS.PL
    ```

---
## 📸 Screenshots & MVC Project Features
 
### 1. Interactive Course Player (Student Workspace)
 
*The unified split-pane student course player that streams lectures, reads articles, and displays assignments without refreshing pages.*
 
| | | |
|---|---|---|
| ![Student Dashboard](https://github.com/ItcProjects-R4/GHR4_SWD5_S2_PROJECT4/blob/cdceb29e3cd5fe56b6a7bda3d38075e6f7261e9c/LMS/screenshots/student_dashboard.png?raw=true) | ![Student Course Player](https://github.com/ItcProjects-R4/GHR4_SWD5_S2_PROJECT4/blob/cdceb29e3cd5fe56b6a7bda3d38075e6f7261e9c/LMS/screenshots/student_course_player.png?raw=true) | ![Student Settings](https://github.com/ItcProjects-R4/GHR4_SWD5_S2_PROJECT4/blob/cdceb29e3cd5fe56b6a7bda3d38075e6f7261e9c/LMS/screenshots/student_settings.png?raw=true) |
 
---
 
### 2. Curriculum Builder (Instructor Workspace)
 
*The step-by-step course builder allowing instructors to add modules, videos, articles, quizzes, and assignments.*
 
| | | |
|---|---|---|
| ![Instructor Dashboard](https://github.com/ItcProjects-R4/GHR4_SWD5_S2_PROJECT4/blob/cdceb29e3cd5fe56b6a7bda3d38075e6f7261e9c/LMS/screenshots/instructor_dashboard.png?raw=true) | ![Instructor Courses List](https://github.com/ItcProjects-R4/GHR4_SWD5_S2_PROJECT4/blob/cdceb29e3cd5fe56b6a7bda3d38075e6f7261e9c/LMS/screenshots/instructor_courses_list.png?raw=true) | ![Curriculum Builder](https://github.com/ItcProjects-R4/GHR4_SWD5_S2_PROJECT4/blob/cdceb29e3cd5fe56b6a7bda3d38075e6f7261e9c/LMS/screenshots/instructor_courses.png?raw=true) |
| ![Create Course Step 1](https://github.com/ItcProjects-R4/GHR4_SWD5_S2_PROJECT4/blob/cdceb29e3cd5fe56b6a7bda3d38075e6f7261e9c/LMS/screenshots/create_course_step1.png?raw=true) | | |
 
---
 
### 3. Staff Delegation & Access Control
 
*Manage instructors, teaching assistants, and permissions through a centralized access-control system.*
 
| | | |
|---|---|---|
| ![Instructor Users Management](https://github.com/ItcProjects-R4/GHR4_SWD5_S2_PROJECT4/blob/cdceb29e3cd5fe56b6a7bda3d38075e6f7261e9c/LMS/screenshots/instructor_users_management.png?raw=true) | ![Instructor Enrollments](https://github.com/ItcProjects-R4/GHR4_SWD5_S2_PROJECT4/blob/cdceb29e3cd5fe56b6a7bda3d38075e6f7261e9c/LMS/screenshots/instructor_enrollments.png?raw=true) | ![Assistant Dashboard](https://github.com/ItcProjects-R4/GHR4_SWD5_S2_PROJECT4/blob/cdceb29e3cd5fe56b6a7bda3d38075e6f7261e9c/LMS/screenshots/assistant_dashboard.png?raw=true) |
| ![Assistant Submissions](https://github.com/ItcProjects-R4/GHR4_SWD5_S2_PROJECT4/blob/cdceb29e3cd5fe56b6a7bda3d38075e6f7261e9c/LMS/screenshots/assistant_submissions.png?raw=true) | ![Grade Submission Workspace](https://github.com/ItcProjects-R4/GHR4_SWD5_S2_PROJECT4/blob/cdceb29e3cd5fe56b6a7bda3d38075e6f7261e9c/LMS/screenshots/grade_submission.png?raw=true) | |
 
---
 
### 4. Financial Ledger & Checkout Tracking
 
*Track payments, purchases, enrollment transactions, and checkout activity in a single financial dashboard.*
 
| | | |
|---|---|---|
| ![Financial Reports Ledger](https://github.com/ItcProjects-R4/GHR4_SWD5_S2_PROJECT4/blob/cdceb29e3cd5fe56b6a7bda3d38075e6f7261e9c/LMS/screenshots/instructor_payments.png?raw=true) | | |
 
---
 
### 5. Public Portal & Discovery
 
*A responsive public-facing website showcasing courses, platform features, instructor information, and enrollment options.*
 
| | | |
|---|---|---|
| ![LMSify Public Landing Page](https://github.com/ItcProjects-R4/GHR4_SWD5_S2_PROJECT4/blob/cdceb29e3cd5fe56b6a7bda3d38075e6f7261e9c/LMS/screenshots/landing_page.png?raw=true) | ![Course Detail Preview](https://github.com/ItcProjects-R4/GHR4_SWD5_S2_PROJECT4/blob/cdceb29e3cd5fe56b6a7bda3d38075e6f7261e9c/LMS/screenshots/course_detail.png?raw=true) | ![Contact Page](https://github.com/ItcProjects-R4/GHR4_SWD5_S2_PROJECT4/blob/cdceb29e3cd5fe56b6a7bda3d38075e6f7261e9c/LMS/screenshots/contact_page.png?raw=true) |
| ![Login Page](https://github.com/ItcProjects-R4/GHR4_SWD5_S2_PROJECT4/blob/cdceb29e3cd5fe56b6a7bda3d38075e6f7261e9c/LMS/screenshots/login_page.png?raw=true) | ![Register Page](https://github.com/ItcProjects-R4/GHR4_SWD5_S2_PROJECT4/blob/cdceb29e3cd5fe56b6a7bda3d38075e6f7261e9c/LMS/screenshots/register_page.png?raw=true) | |

---

## 🏗️ Architecture & File Directory Explanation

The solution is structured using **Clean Architecture** patterns. Here is the layout of the project components:

```text
LMS/ (Solution Root)
├── LMS.Domain/                # Core Enterprise Layer (No dependencies)
│   ├── Enums/                 # Global enums (e.g. PaymentStatus, EnrollmentStatus, SubmissionStatus)
│   └── Models/                # Database entities (e.g. Course, Module, Content, Payment, Enrollment)
├── LMS.DAL/                   # Data Access Layer
│   ├── Data/                  # DbContext schema and DbInitializer seed classes
│   ├── Migrations/            # EF Core schema migration logs
│   └── Repositories/          # Repositories executing data queries (e.g. PaymentRepository)
├── LMS.BLL/                   # Business Logic Layer
│   ├── Services/              # Third-party integrations (Cloudinary, Paymob) and services
│   └── ViewModels/            # Bindable view models for Razor views
└── LMS.PL/                    # Presentation Layer (MVC Web Application)
    ├── Controllers/           # MVC Controllers mapping requests (e.g. CheckoutController)
    ├── Views/                 # Server-side HTML Razor views
    └── wwwroot/               # Static styles (index.css), custom JS, and graphic files
```

---

## ⚠️ Challenges Encountered

*   **Paymob HMAC Verification (Type Normalization):** Paymob requires alphabetical string concatenation of callback values before generating the HMAC hash. We discovered that standard .NET JsonElement.ToString() outputs boolean fields as title-case ("True"/"False"), causing verification to mismatch. We resolved this by building a normalizer converting booleans strictly to lowercase strings ("true"/"false") before concatenation.
*   **Double Submission Risk:** If a student clicks the checkout pay button multiple times, the server initiates parallel requests creating duplicate pending records. We implemented front-end script listeners disabling the submit button instantly on click.
*   **Localhost Webhook Processing:** Because local developers are behind NAT firewalls, Paymob webhooks cannot reach localhost. We created a development bypass in the controller actions to simulate payment completions during local integration testing.

---

## 🔮 Future Roadmap & Enhancements

*   **AI-Generated Learning Paths:** Implement recommendations adapting content to individual student test scores.
*   **Automated Certificate Generation:** Generate verified PDF certificates with verification QR codes upon course completion.
*   **Live Web Seminar Integration:** Embed Zoom/Teams API connections for live online workshops.
*   **Gamification Systems:** Introduce reward badges and weekly learning streaks to boost retention.

---

## 👥 Team Members

*   **Shahd Muhammed Gaballah** - Team Leader
*   **Amira Khamis Hendawy** - Team Member
*   **Hoda Ahmed Elgarm** - Team Member
*   **Moataz Ahmed Gado** - Team Member
*   **Mohamed Ashraf Shaheen** - Team Member
*   **Mostafa Yasser Meshemsh** - Team Member
---

## 🎥 Presentation & Explainer Video Links

*   **Live Hosted Platform:** [LMSify Live Portal](https://lms-platform.runasp.net)
*   **Walkthrough Explainer Video:** [Watch Explainer Video on YouTube]() 
*   **Documentation:** [View Google Slides / PDF Presentation]()
```
