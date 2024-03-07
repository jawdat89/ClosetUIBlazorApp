export function drawParts(canvasId, paramsModel, dotnet) {
    try {
        var canvas = document.getElementById(canvasId);
        if (!canvas) {
            console.error('Canvas element not found');
            return;
        }
        var ctx = canvas.getContext('2d');
        ctx.clearRect(0, 0, canvas.width, canvas.height);

        var parts = paramsModel.parts;
        var currentX = 0, currentY = 0, maxHeightInRow = 0, boardOffsetX = 0;
        var boardWidth = paramsModel.totalWidth;

        for (var i = 0; i < parts.length; i++) {
            var part = parts[i];
            var color = generateRandomColor(); // One color per part type

            for (var qty = 0; qty < part.partQty; qty++) {
                var pos = calculateNextPosition(part, currentX, currentY, maxHeightInRow, boardOffsetX, boardWidth, paramsModel.totalHeight);
                currentX = pos.currentX;
                currentY = pos.currentY;
                maxHeightInRow = pos.maxHeightInRow;
                boardOffsetX = pos.boardOffsetX;

                drawPart(ctx, part, currentX, currentY, color);

                // Update the x position for the next part instance
                currentX += part.wt;
                // Update the tallest part in the current row
                maxHeightInRow = Math.max(maxHeightInRow, part.ht);
            }

            // Reset for next part type, if needed
            if (currentX + part.wt > boardWidth) {
                currentY += maxHeightInRow;
                currentX = boardOffsetX; // May need adjusting based on your intended behavior
                maxHeightInRow = 0;
            }
        }

        dotnet.invokeMethodAsync('OnCanvasSuccess', true);

    } catch (e) {
        console.error(e);
        dotnet.invokeMethodAsync('OnCanvasError', 'Error drawing parts: ' + e.message);
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

        // Assuming you want to apply the zoom effect immediately after setting the canvas size
        const ctx = canvas.getContext('2d');
        applyCanvasZoom(ctx, 0.5); // Apply a 50% zoom out effect
    } catch (e) {
        handleError(e, dotnet);
    }
}

function generateRandomColor() {
    var r = Math.floor(Math.random() * 256);
    var g = Math.floor(Math.random() * 256);
    var b = Math.floor(Math.random() * 256);
    return 'rgba(' + r + ', ' + g + ', ' + b + ', 0.5)'; // Semi-transparent color
}

function calculateNextPosition(part, currentX, currentY, maxHeightInRow, boardOffsetX, boardWidth, totalHeight) {
    if (currentX + part.wt > boardWidth + boardOffsetX) {
        currentY += maxHeightInRow; // Move to the next row
        currentX = boardOffsetX; // Reset currentX to the start of the current board
        maxHeightInRow = 0;
    }
    if (currentY + part.ht > totalHeight) {
        boardOffsetX += boardWidth; // Start a new board
        currentX = boardOffsetX; // Reset currentX to the start of the new board
        currentY = 0; // Reset Y position for the new board
    }
    return { currentX, currentY, maxHeightInRow, boardOffsetX };
}

function drawPart(ctx, part, currentX, currentY, color) {
    // Draw the part as before
    ctx.fillStyle = color;
    ctx.fillRect(currentX, currentY, part.wt, part.ht);
    ctx.strokeStyle = 'black';
    ctx.lineWidth = 1;
    ctx.strokeRect(currentX, currentY, part.wt, part.ht);

    // Set text properties
    ctx.fillStyle = 'black'; // Text color
    ctx.font = '55px Arial'; // Text size and font
    ctx.textBaseline = 'top'; // Align text vertically

    // Draw partWidth at the top line
    ctx.fillText(part.partWidth + 'mm', currentX + 4, currentY); // Adjust as needed for positioning

    // Change text baseline for side drawing
    ctx.textBaseline = 'bottom';
    ctx.save(); // Save context to reset later
    ctx.translate(currentX, currentY + part.ht);
    ctx.rotate(-Math.PI / 2); // Rotate text for left side
    ctx.fillText(part.partHeight + 'mm', 4, 0); // Adjust positioning
    ctx.restore(); // Restore context to previous state
}


function applyCanvasZoom(ctx, scaleFactor) {
    // Apply a transform to scale everything down by the specified scaleFactor
    // This affects the "world" of the canvas, making everything drawn to it scaled.
    ctx.setTransform(scaleFactor, 0, 0, scaleFactor, 0, 0); // Apply scaling
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