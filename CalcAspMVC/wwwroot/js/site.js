// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.



// Clear Display
function clearDisplay() {
    $("#display").val("");
}

// Switch Calculator Theme
function switchTheme(value) {
    // Update body class
    const body = document.body;
    body.className = value + '-mode'
    // Update theme label text
    document.getElementById("themeLbl").innerHTML = value.toUpperCase() + " THEME";
}

// Validate Operators
function validateOperator(value) {
    // List of all operators
    const substrings = ['+', '*', '.', '/', '-'];
    // Determine if value is found in operators
    return (substrings.some(v => value === v));
}

// Valididate Open Parentheses
function validateParentheses(input) {

    // Get value from display
    let str = $("#display").val();
    // Get last character from display
    let lastExpression = str[str.length - 1];

    // Store edits made parentheses
    let org = input;

    if (str) {

        if (input == '(') {

            // Determine if previous input is an operator or parentheses
            if (validateOperator(lastExpression) || lastExpression == input) {
                org = '(';
            }
            else {
                org = '*(';
            }
        }
    }

    return org;
}

// Validate Closed Parentheses
function validateCloseParentheses() {

    // Get value from display
    let str = $("#display").val();

    // Count both opening & closing parentheses
    const a = str.split('(').length - 1;
    const b = str.split(')').length - 1;

    // Needs while loop
    if (a > b) {
        // Close open parentheses
        $("#display").val(
            str + ')');
    }
}

// Validate Input
function validateInput(input) {
    // Get previous input from display
    let str = $("#display").val();
    let lastExpression = str[str.length - 1];

    // Check if current input is an operator
    if (validateOperator(input)) {

        // If display is empty
        // Then only allow (Plus & Minus) operators
        if (str == "" && !(input == "+" || input == "-")) {
            return false;
        }

        // Cannot add double Operators
        // Check previous input to avoid double operators
        if (validateOperator(lastExpression)) {
            return false;
        }
    }

    return true;
}


// Adjust MemoryStore Modal Position
function adjustModalPosition() {

    // Get dimension of the calculator div
    var calculatorContainer = document.getElementById('calcBody');
    var calculatorRect = calculatorContainer.getBoundingClientRect();

    // Update modelDialog dimensions to align with the calculator div
    var modalDialog = document.querySelector('.modal-dialog-bottom');
    modalDialog.style.width = calculatorRect.width + 'px';
    modalDialog.style.left = calculatorRect.left + 'px';
}

// Display the MemoryStore 
function showMemory() {
    $('#memoryModal').modal('show');
}

// Populate MemoryStore Modal
function updateMemoryList(previousCalculations) {

    // Clear previous entries from Modal
    var memoryList = $('#memory-list');
    memoryList.empty();

    // Keep track of current index
    var index = 0;

    // Create new Div Element for each entry in MemoryStore
    previousCalculations.forEach(function (calculation) {

        // Build HTML Template for entry
        var listItem = `<li class="list-group-item d-flex justify-content-between align-items-center">
                                    ${calculation}
                                    <span>
                                    <button type="button" class="dynamic_button btn btn-danger btn-sm" value="${index}" data-action="MemoryClear">MC</button>
                                    <button type="button" class="dynamic_button btn btn-success btn-sm" value="${index}" data-action="MemoryAdd">M+</button>
                                    <button type="button" class="dynamic_button btn btn-warning btn-sm" value="${index}" data-action="MemorySubstract">M-</button>
                                    </span>
                                </li>`;

        // Append Html Template to MemoryStore Modal
        memoryList.append(listItem);
        // Update index
        index++;
    });
}