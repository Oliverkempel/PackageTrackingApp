/** @type {import('tailwindcss').Config} */
module.exports = {
    content: ["./Views/**/*.{html,cshtml,js}"],
  theme: {
      extend: {
          colors: {
              primaryBlue: '#3c99b6',
              secondaryBlue: '#6da5ff',
              primaryGray: '#eeeeee',
              primaryYellow: '#fff1dc'

          }
      },
  },
  plugins: [],
}
