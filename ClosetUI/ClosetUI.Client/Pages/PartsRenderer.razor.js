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
    ctx.fillStyle = color;
    ctx.fillRect(currentX, currentY, part.wt, part.ht);
    ctx.strokeStyle = 'black';
    ctx.lineWidth = 1;
    ctx.strokeRect(currentX, currentY, part.wt, part.ht);
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