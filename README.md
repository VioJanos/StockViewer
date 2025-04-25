# StockViewer 📈

**StockViewer** is a Single Page Application (SPA) built with F# and WebSharper. It fetches and displays real-time stock prices across different time intervals and data providers. The system automatically switches API keys and falls back to a secondary provider when usage limits are reached.

## 🔧 Features

- ✅ Dropdown menu for selecting a stock (e.g., IBM, AAPL, etc.)
- 📊 Display of stock prices:
  - Hourly, daily, weekly, and monthly intervals
- 🔁 Automatic API key rotation for Alpha Vantage (fallback to TwelveData)
- 🧾 Export stock data to CSV
- 🔗 Persistent navigation bar between pages

## 🛠 Technologies Used

- F# programming language
- [WebSharper](https://websharper.com/) SPA framework
- Alpha Vantage and TwelveData API integration
- WebSharper.Charting for chart visualization
- Custom HTML/CSS via `Style.fs`

## 🚀 Getting Started

### Development

```bash
dotnet tool restore
dotnet run
```

The app will be available at `http://localhost:5000`.

### Deployment (Render or GitHub Pages)

This project supports auto-deployment using Render.com or GitHub Actions.  
See `.github/workflows/deploy.yml` for the CI/CD pipeline.

#### Live Demo  
👉 [Try it live!](https://stockviewer-oi64.onrender.com)

## 📁 Project Structure

```
StockViewer/
│
├── Client/             # Client-side F# logic
├── Server/             # Server-side API integration
├── Shared/             # Shared types (e.g., StockDataPoint)
├── wwwroot/            # Static assets
├── style.fs            # Styling definitions
├── StockViewer.fsproj  # Project file
└── README.md           # This file
```

## 📸 Screenshots

### 🏠 Home Page
![Home Page](screenshots/Home_page.png)

### 📈 Stock Chart View Before Data Loaded
![Stock Chart Before Data](screenshots/Stock_chart_before_data.png)

### 📈 Stock Chart View After Data Loaded
![Stock Chart After Data](screenshots/Stock_chart_after_data.png)
