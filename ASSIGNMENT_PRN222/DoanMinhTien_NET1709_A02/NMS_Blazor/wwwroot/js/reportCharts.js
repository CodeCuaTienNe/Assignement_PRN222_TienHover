// Global chart objects for cleanup
let dailyChart = null;
let statusChart = null;
let categoryChart = null;
let authorChart = null;

// Consolidated function to render all charts at once
function renderAllCharts(
  dailyLabels,
  dailyData,
  statusLabels,
  statusData,
  categoryLabels,
  categoryData,
  authorLabels,
  authorData
) {
  console.log("Starting chart rendering...");

  // Delay chart rendering slightly to ensure DOM elements are ready
  setTimeout(() => {
    try {
      renderDailyChart(dailyLabels, dailyData);
      renderStatusChart(statusLabels, statusData);
      renderCategoryChart(categoryLabels, categoryData);
      renderAuthorChart(authorLabels, authorData);
      console.log("All charts rendered successfully!");
    } catch (error) {
      console.error("Error rendering charts:", error);
    }
  }, 250);
}

// Render the daily chart (line chart)
function renderDailyChart(labels, data) {
  console.log("Rendering daily chart...");
  const canvas = document.getElementById("dailyChart");

  if (!canvas) {
    console.error("Daily chart canvas element not found!");
    return;
  }

  const ctx = canvas.getContext("2d");

  // Destroy previous chart instance if exists
  if (dailyChart) {
    dailyChart.destroy();
  }

  // Create new chart
  dailyChart = new Chart(ctx, {
    type: "line",
    data: {
      labels: labels,
      datasets: [
        {
          label: "Articles Published",
          data: data,
          backgroundColor: "rgba(75, 192, 192, 0.2)",
          borderColor: "rgba(75, 192, 192, 1)",
          borderWidth: 2,
          tension: 0.1,
          fill: true,
        },
      ],
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      scales: {
        y: {
          beginAtZero: true,
          ticks: {
            precision: 0, // Integer values only
          },
        },
      },
      plugins: {
        legend: {
          display: true,
          position: "top",
        },
        tooltip: {
          mode: "index",
          intersect: false,
        },
      },
    },
  });
}

// Render the status chart (pie chart)
function renderStatusChart(labels, data) {
  console.log("Rendering status chart...");
  const canvas = document.getElementById("statusChart");

  if (!canvas) {
    console.error("Status chart canvas element not found!");
    return;
  }

  const ctx = canvas.getContext("2d");

  // Destroy previous chart instance if exists
  if (statusChart) {
    statusChart.destroy();
  }

  // Colors for status (Green for active, gray for inactive)
  const colors = ["rgba(40, 167, 69, 0.8)", "rgba(108, 117, 125, 0.8)"];

  // Create new chart
  statusChart = new Chart(ctx, {
    type: "pie",
    data: {
      labels: labels,
      datasets: [
        {
          data: data,
          backgroundColor: colors,
          borderWidth: 1,
        },
      ],
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: {
          position: "top",
        },
        tooltip: {
          callbacks: {
            label: function (context) {
              let label = context.label || "";
              let value = context.raw || 0;
              let sum = 0;

              // Calculate sum
              context.dataset.data.forEach((d) => (sum += d));

              // Calculate percentage
              let percentage =
                sum > 0 ? ((value / sum) * 100).toFixed(1) + "%" : "0%";

              return `${label}: ${value} (${percentage})`;
            },
          },
        },
      },
    },
  });
}

// Render the category chart (bar chart)
function renderCategoryChart(labels, data) {
  console.log("Rendering category chart...");
  const canvas = document.getElementById("categoryChart");

  if (!canvas) {
    console.error("Category chart canvas element not found!");
    return;
  }

  const ctx = canvas.getContext("2d");

  // Destroy previous chart instance if exists
  if (categoryChart) {
    categoryChart.destroy();
  }

  // Generate random colors for categories
  const colors = generateRandomColors(labels.length);

  // Create new chart
  categoryChart = new Chart(ctx, {
    type: "bar",
    data: {
      labels: labels,
      datasets: [
        {
          label: "Articles",
          data: data,
          backgroundColor: colors,
          borderWidth: 1,
        },
      ],
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      scales: {
        y: {
          beginAtZero: true,
          ticks: {
            precision: 0, // Integer values only
          },
        },
      },
      plugins: {
        legend: {
          display: false,
        },
        title: {
          display: true,
          text: "Top Categories by Article Count",
        },
      },
    },
  });
}

// Render the author chart (horizontal bar chart)
function renderAuthorChart(labels, data) {
  console.log("Rendering author chart...");
  const canvas = document.getElementById("authorChart");

  if (!canvas) {
    console.error("Author chart canvas element not found!");
    return;
  }

  const ctx = canvas.getContext("2d");

  // Destroy previous chart instance if exists
  if (authorChart) {
    authorChart.destroy();
  }

  // Generate random colors for authors
  const colors = generateRandomColors(labels.length);

  // Create new chart
  authorChart = new Chart(ctx, {
    type: "bar",
    data: {
      labels: labels,
      datasets: [
        {
          label: "Articles",
          data: data,
          backgroundColor: colors,
          borderWidth: 1,
        },
      ],
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      indexAxis: "y", // Horizontal bar chart
      scales: {
        x: {
          beginAtZero: true,
          ticks: {
            precision: 0, // Integer values only
          },
        },
      },
      plugins: {
        legend: {
          display: false,
        },
        title: {
          display: true,
          text: "Top Authors by Article Count",
        },
      },
    },
  });
}

// Generate random colors for charts
function generateRandomColors(count) {
  const colors = [];

  for (let i = 0; i < count; i++) {
    const r = 50 + Math.floor(Math.random() * 150);
    const g = 50 + Math.floor(Math.random() * 150);
    const b = 50 + Math.floor(Math.random() * 150);
    colors.push(`rgba(${r}, ${g}, ${b}, 0.7)`);
  }

  return colors;
}

// Export table to CSV
function exportTableToCSV(filename) {
  const table = document.getElementById("articlesTable");
  let csv = [];

  // Get all rows
  const rows = table.querySelectorAll("tr");

  for (const row of rows) {
    // Get all cells in this row
    const cells = row.querySelectorAll("td, th");
    const rowData = [];

    for (const cell of cells) {
      // Replace double quotes with two double quotes and wrap with quotes
      rowData.push('"' + (cell.textContent || "").replace(/"/g, '""') + '"');
    }

    // Add row to CSV
    csv.push(rowData.join(","));
  }

  // Create CSV content
  const csvContent = csv.join("\n");

  // Create download link
  const encodedUri =
    "data:text/csv;charset=utf-8," + encodeURIComponent(csvContent);
  const link = document.createElement("a");
  link.setAttribute("href", encodedUri);
  link.setAttribute("download", filename);
  document.body.appendChild(link);

  // Trigger download
  link.click();
  document.body.removeChild(link);
}

// Export report to PDF
function exportReportToPDF(filename) {
  const reportElement = document.getElementById("report-content");
  const originalOverflow = document.body.style.overflow;

  // Temporarily adjust styling for PDF capturing
  document.body.style.overflow = "visible";

  html2pdf()
    .set({
      margin: 10,
      filename: filename + ".pdf",
      image: { type: "jpeg", quality: 0.98 },
      html2canvas: { scale: 2 },
      jsPDF: { unit: "mm", format: "a4", orientation: "portrait" },
    })
    .from(reportElement)
    .save()
    .then(() => {
      // Restore original styling
      document.body.style.overflow = originalOverflow;
    });
}

// Print report
function printReport() {
  window.print();
}
