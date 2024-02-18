/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ["./**/*.{html,js,css,razor}"],
  theme: {
    extend: {},
  },
  plugins: [require("daisyui")],
  daisyui: {
    themes: [
      {
        light: {
          "primary": "#4D6AFF",
          "primary-content": "#fff",
          "neutral": "#dddddd",
          "neutral-content": "#888",
          
          "base-100": "#e7f2fc",
          "base-200": "#ffffff",
          "base-300": "#fff",
          "base-content": "#000",
          
          "info": "#4DE1FF",
          "success": "#A6FF4D",
          "warning": "#FFA64D",
          "error": "#FF4D4D",
        },
        dark: {
          "primary": "#647dff",
          "primary-content": "#fff",
          
          "neutral": "#242c36",
          "neutral-content": "#ccc",
          
          "base-100": "#161b21",
          "base-200": "#1d232c",
          "base-300": "#364151",
          "base-content": "#fff",
          
          "info": "#4DE1FF",
          "success": "#A6FF4D",
          "warning": "#FFA64D",
          "error": "#ff6464",
          "error-content": "#FF4D4D"
        }
      },
    ],
  },
}

