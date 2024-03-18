export function saveAsFile(fileName, byteBase64) {
    const blob = new Blob([new Uint8Array(byteBase64)], { type: "application/pdf" });
    const link = document.createElement("a");
    link.href = window.URL.createObjectURL(blob);
    link.download = fileName;
    document.body.appendChild(link); // Required for Firefox
    link.click();
    document.body.removeChild(link);
}