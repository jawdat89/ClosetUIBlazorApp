export function drawParts(canvasId, paramsModel, dotnet) {
    try {
        var canvas = document.getElementById(canvasId);
        if (!canvas) {
            console.error('Canvas element not found');
            dotnet.invokeMethodAsync('OnCanvasError', 'Canvas element not found');
            return;
        }
        var ctx = canvas.getContext('2d');
        ctx.clearRect(0, 0, canvas.width, canvas.height);

        // The logical layout is based on original dimensions from paramsModel
        var parts = paramsModel.parts;
        var currentX = 0, currentY = 0, maxHeightInRow = 0, boardOffsetX = 0;
        var boardWidth = paramsModel.totalWidth;
        var boardHeight = paramsModel.totalHeight;
        var scaleFactor = 1; // Assuming no visual scaling or adjust as needed

        // Initial board dimensions drawing
        drawBoardDimensions(ctx, boardOffsetX, 0, boardWidth, boardHeight, scaleFactor);

        for (var i = 0; i < parts.length; i++) {
            var part = parts[i];
            var partColor = generateRandomColor();

            for (var qty = 0; qty < part.partQty; qty++) {
                // Visual dimensions might be different, but logical checks use original dimensions
                if (currentX + part.wt > boardWidth) {
                    // Move to the next row
                    currentY += maxHeightInRow;
                    currentX = 0;
                    maxHeightInRow = 0;
                }
                if (currentY + part.ht > paramsModel.totalHeight) {
                    // Start a new board, reset X, and Y, and apply visual board offset
                    boardOffsetX += boardWidth + 10; // Spacing between boards
                    currentX = 0;
                    currentY = 0;

                    drawBoardDimensions(ctx, boardOffsetX, 0, boardWidth, boardHeight, scaleFactor);
                }

                // Visual scaling for drawing, not affecting logical placement
                var visualX = (currentX + boardOffsetX) * scaleFactor;
                var visualY = currentY * scaleFactor;
                var visualWt = part.wt * scaleFactor;
                var visualHt = part.ht * scaleFactor;

                drawPart(ctx, part, visualX, visualY, partColor, visualWt, visualHt);

                // Update positions for the next part using original dimensions
                currentX += part.wt;
                maxHeightInRow = Math.max(maxHeightInRow, part.ht);
            }
        }

        dotnet.invokeMethodAsync('OnCanvasSuccess', true);
    } catch (e) {
        console.error('Error drawing parts: ' + e.message);
        dotnet.invokeMethodAsync('OnCanvasError', 'Error drawing parts: ' + e.message);
    }
}

export function setCanvasSize(canvasId, dotnet) {
    try {
        const canvas = document.getElementById(canvasId);

        if (!canvas) return;

        canvas.style.backgroundColor = 'rgba(0,0,0,0.05)';

        // Determine the available space while keeping some margins from the window's dimensions
        const maxWidth = window.innerWidth * 0.85;
        const maxHeight = window.innerHeight * 0.85;

        // Get the canvas's inherent aspect ratio if it has defined intrinsic dimensions
        const intrinsicWidth = canvas.getAttribute('data-intrinsic-width');
        const intrinsicHeight = canvas.getAttribute('data-intrinsic-height');
        let aspectRatio = intrinsicWidth && intrinsicHeight ? intrinsicWidth / intrinsicHeight : maxWidth / maxHeight;

        // Calculate potential dimensions
        let potentialWidth = maxHeight * aspectRatio;
        let potentialHeight = maxWidth / aspectRatio;

        // Decide on the final size based on the potential dimensions fitting within the max constraints
        if (potentialWidth <= maxWidth && potentialHeight <= maxHeight) {
            canvas.width = potentialWidth;
            canvas.height = maxHeight;
        } else {
            canvas.width = maxWidth;
            canvas.height = potentialHeight;
        }

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

function drawPart(ctx, part, currentX, currentY, color, visualWt, visualHt) {
    // Draw the part as before
    ctx.fillStyle = color;
    ctx.fillRect(currentX, currentY, visualWt, visualHt);
    ctx.strokeStyle = 'black';
    ctx.lineWidth = 1;
    ctx.strokeRect(currentX, currentY, visualWt, visualHt);

    // Set text properties
    ctx.fillStyle = 'black'; // Text color
    ctx.font = '22px Arial'; // Text size and font
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

function drawBoardDimensions(ctx, offsetX, offsetY, width, height, scaleFactor) {
    var visualWidth = width * scaleFactor;
    var visualHeight = height * scaleFactor;

    ctx.strokeStyle = "#3C3732FF"; // Color for board dimensions outline
    ctx.lineWidth = 2; // Line width for the outline
    ctx.strokeRect(offsetX, offsetY, visualWidth, visualHeight); // Draw the outline for each board
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