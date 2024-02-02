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
        mytheme: {
          "primary": "#4D6AFF",
          "secondary": "#ffffff",
          "neutral-content": "#888",
          "accent": "#ffffff",
          "neutral": "#dddddd",
          "base-100": "#e7f2fc",
          "info": "#4DE1FF",
          "success": "#A6FF4D",
          "warning": "#FFA64D",
          "error": "#FF4D4D",
        },
      },
    ],
  },
}

