export function drawParts(canvasId, parts, dotnet) {
    try {
        var canvas = document.getElementById(canvasId);
        if (canvas.getContext) {
            var ctx = canvas.getContext('2d');

            let currentX = 0; // Start position for X
            let currentY = 0; // Start position for Y
            let rowHeight = 0; // Height of the current row, to adjust Y when needed

            for (let i = 0; i < parts.length; i++) {
                let part = parts[i];

                if (currentX + part.Wt > canvas.width) { // Check if part fits in the current row
                    currentX = 0; // Reset X to start new row
                    currentY += rowHeight; // Move Y down to next row
                    rowHeight = 0; // Reset row height for the new row
                }

                // Update row height if current part's height is greater than the current max row height
                if (part.Ht > rowHeight) {
                    rowHeight = part.Ht;
                }

                // Check if there's enough vertical space to place the part
                if (currentY + part.Ht > canvas.height) {
                    console.error("Not enough space to place all parts");
                    break; // Stop drawing if we run out of vertical space
                }

                // Generate a random color
                var r = Math.floor(Math.random() * 256);
                var g = Math.floor(Math.random() * 256);
                var b = Math.floor(Math.random() * 256);
                ctx.fillStyle = `rgba(${r}, ${g}, ${b}, 0.5)`;
                ctx.strokeStyle = 'black';
                ctx.lineWidth = 2;

                // Draw part
                ctx.fillRect(currentX, currentY, part.Wt, part.Ht);
                ctx.strokeRect(currentX, currentY, part.Wt, part.Ht);

                // Update X for the next part
                currentX += part.Wt;
            }
        }
    } catch (e) {
        handleError(e, dotnet);
    }
}

export function setCanvasSize(canvasId, dotnet) {
    try {
        const canvas = document.getElementById(canvasId);


        if (!canvas) return;

        canvas.style.backgroundColor = 'rgba(0,0,0,0.05)';

        // Example: Set the canvas width to 90% of the window width and maintain a 16:9 aspect ratio
        const maxWidth = window.innerWidth * 0.85; // 90% of the window width
        const maxHeight = maxWidth * (9 / 16); // Maintain a 16:9 aspect ratio

        // You might want to add logic here to not exceed the original TotalWidth and TotalHeight
        // if that's important for your application's functionality.

        canvas.width = maxWidth;
        canvas.height = maxHeight;
    } catch (e) {
        handleError(e, dotnet);
    }
}

function handleError(error, dotnet) {
    if (error) {
        errorMsg(`Canvas Error: ${error}`, dotnet);
    }
}

function errorMsg(msg, dotnet) {
    if (typeof msg !== 'undefined') {
        console.error(msg);
        dotnet.invokeMethodAsync("OnCanvasError", msg);
    }
}