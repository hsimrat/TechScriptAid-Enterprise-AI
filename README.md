## 📺 Video Tutorials

- [Episode 1: Building an Enterprise API with Clean Architecture](https://youtu.be/YOUR-VIDEO-ID)
  - Source code: [Episode 1 Branch](https://github.com/hsimrat/TechScriptAid-Enterprise-AI/tree/episode-1)
  - Source code: [Episode 2 Branch](https://github.com/hsimrat/TechScriptAid-Enterprise-AI/tree/episode-2-ef-core-migrations)
    


# TechScriptAid Enterprise AI Development

Welcome to the official code repository for the TechScriptAid YouTube channel! This repository contains all the code from our tutorial series on building production-ready enterprise applications with .NET and AI.

## 🎯 About This Series

Learn how to build enterprise-grade applications combining:
- **.NET 8** - Latest features and best practices
- **Clean Architecture** - Maintainable and testable code
- **Azure AI Services** - Integration with OpenAI and Cognitive Services
- **Real-world patterns** - From 17+ years of enterprise experience

## 📁 Project Structure
TechScriptAid.EnterpriseAI/
├── src/
│   ├── TechScriptAid.API/           # Web API Layer
│   ├── TechScriptAid.Core/          # Domain Entities & Interfaces
│   ├── TechScriptAid.Infrastructure/# Data Access & External Services
│   └── TechScriptAid.AI/            # AI Integration Layer
├── tests/                           # Unit and Integration Tests
├── docs/                            # Documentation
│   └── episodes/                    # Episode-specific guides
└── samples/                         # Code samples and demos

## 🚀 Getting Started

### Prerequisites
- .NET 8 SDK
- Visual Studio 2022 or VS Code
- Azure Account (free tier works)
- Git

Quick Start
bash# Clone the repository
git clone https://github.com/hsimrat/TechScriptAid-Enterprise-AI.git

# Checkout Episode 2 branch
git checkout episode-2-ef-core-migrations

# Navigate to the project
cd TechScriptAid-Enterprise-AI

# Restore dependencies
dotnet restore

# Update database (Episode 2)
dotnet ef database update --project src/TechScriptAid.Infrastructure --startup-project src/TechScriptAid.API

# Run the API
dotnet run --project src/TechScriptAid.API
📺 Episodes

✅ Episode 001 - Setting Up Enterprise Development Environment
🆕 Episode 002 - Entity Framework Core Migrations & Advanced Data Operations
🔜 Episode 003 - Adding Azure OpenAI Integration (Coming Soon)
🔜 Episode 004 - Implementing CQRS Pattern (Coming Soon)


🛠️ Technologies Used

Backend: .NET 8, ASP.NET Core, C# 12
Architecture: Clean Architecture, CQRS, Repository Pattern
AI/ML: Azure OpenAI, Semantic Kernel, ML.NET
Database: SQL Server, Entity Framework Core
Testing: xUnit, Moq, FluentAssertions
CI/CD: GitHub Actions, Azure DevOps

🔧 Episode 2: What You'll Learn
In this episode, we cover:

Setting up Entity Framework Core in Clean Architecture
Creating and managing database migrations
Implementing Repository and Unit of Work patterns
Advanced querying techniques
Database seeding and initialization
Handling database relationships
Performance optimization tips

Key Commands from Episode 2
powershell# Add a new migration
dotnet ef migrations add InitialCreate --project src/TechScriptAid.Infrastructure --startup-project src/TechScriptAid.API

# Update database
dotnet ef database update --project src/TechScriptAid.Infrastructure --startup-project src/TechScriptAid.API

# Remove last migration
dotnet ef migrations remove --project src/TechScriptAid.Infrastructure --startup-project src/TechScriptAid.API

# Generate SQL script
dotnet ef migrations script --project src/TechScriptAid.Infrastructure --startup-project src/Tech

📧 Connect

YouTube: TechScriptAid Channel
GitHub: @hsimrat
LinkedIn: https://www.linkedin.com/in/harsimrat


⭐ If you find this helpful, please star the repository and subscribe to the channel!
