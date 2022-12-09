function validateInput(input, elementToDisable) {
    document.addEventListener("DOMContentLoaded", () => {
        disableElementIfEmptyInput(input, elementToDisable);
    });

    input.addEventListener("input", () => {
        disableElementIfEmptyInput(input, elementToDisable);
    });
}

function disableElementIfEmptyInput(input, element) {
    const DISABLED = "disabled";

    const value = input.value;

    if (isEmpty(value)) {
        element.setAttribute(DISABLED, true);
    }
    else {
        element.removeAttribute(DISABLED);
    }
}

function isEmpty(str) {
    return !str.trim().length;
}
