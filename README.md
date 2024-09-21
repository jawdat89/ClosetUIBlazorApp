# ClosetUIBlazorApp

## Project Description
ClosetUIBlazorApp is a Blazor application designed to optimize the cutting of MDF boards. It helps users plan and execute cuts in the most efficient way possible, minimizing waste and maximizing the use of materials. This tool is ideal for woodworkers, carpenters, and DIY enthusiasts who work with MDF boards.

## Features
- Plan and visualize MDF board cuts.
- Optimize cutting patterns to reduce waste.
- Support for both straight and curved cuts.
- User-friendly interface for easy navigation and planning.
- Save and load cutting plans for future use.
- Supports multiple languages: English (en) and Hebrew (he).

## Technologies Used
- **Blazor.Extensions.Canvas** (Version 1.1.1): Used for drawing the final parts on a canvas in the client Blazor component.
- **Blazored.LocalStorage** (Version 4.5.0): Utilized for storing user data locally.

## Canvas Helper Credit
The canvas drawing functionality is adapted from Scott Harden's excellent blog post, ["Draw Animated Graphics in the Browser with Blazor WebAssembly"](https://swharden.com/blog/2021-01-07-blazor-canvas-animated-graphics/).

## Table of Contents
- [Installation](#installation)
- [Usage](#usage)
- [Contributing](#contributing)
- [License](#license)
- [Contact](#contact)

## Installation
To run this project locally, follow these steps:

1. Clone the repository:
`bash
git clone https://github.com/jawdat89/ClosetUIBlazorApp.git
`
2. Navigate to the project directory:
`bash
cd ClosetUIBlazorApp
`
3. Install the necessary dependencies:
`bash
dotnet restore
`
4. Run the application:
`bash
dotnet run
`
Or use MS Visual Studio

## Usage
Once the application is running, you can access it in your web browser at `http://localhost:5000`. Use the interface to input the dimensions of your MDF boards and the required pieces. The app will generate an optimized cutting plan.

## Contributing
Contributions are welcome! Please follow these steps to contribute:

1. Fork the repository.
2. Create a new branch:
`bash
git checkout -b feature-name
`
3. Make your changes and commit them:
`bash
git commit -m "Description of changes"
`
4. Push to the branch:
`bash
git push origin feature-name
`
5. Open a pull request.

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.

## Contact
For any questions or suggestions, please contact Jawdat Abdullah at Jawdat.89@gmail.com.
