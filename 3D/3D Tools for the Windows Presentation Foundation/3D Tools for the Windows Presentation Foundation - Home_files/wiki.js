function openPreviewWikiWindow(url) {
    newWindow = window.open(url, 'Print', 'toolbar=0, menubar=0, location=0, status=0, scrollbars=1, resizable=1');
}

function printWiki() {
    parent.print()
    //parent.frames[1].focus();
    //parent.frames[1].print();
}

function closeWiki() {
    parent.focus();
    parent.close();
}