function buttonClick() {
    const anchor = document.createElement("a");
    anchor.href = "https://localhost:7248/Apps/0/download";
    anchor.download = "backup.zip";

    document.body.appendChild(anchor);
    anchor.click();
    document.body.removeChild(anchor);
}