// The characters that may be displayed within each text block
var characters = new Array( "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", 
                            "r", "s", "t", "u", "v", "w", "x", "y", "z", "A", "B", "C", "D", "E", "F", "G", "H",
                            "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y",
                            "Z", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "!", "@", "#", "$", "%", "^",
                            "&", "*", "(", ")", "-", "+", "[", "]", "/", "?");

// Shades of green available for use within the animation                     
var greens = new Array("#008000", "#228B22", "#32CD32", "#808000", "#9ACD32");

// The entered text
var textValue = new Array();
var textToDisplay = new String();
var columnSet = new Array();

var offset = parseInt("0");
var opacity = parseFloat("1.0");

// The recursiveHandlers
var recurse0;
var recurse1;
var recurse2;

// A handle to the WPF/E control
var activeXControl = null;
              
// The total number of characters within the display              
var TOTAL_CHARACTERS = 140;
var TOTAL_COLUMNS = 20;

// Specifies the number of milliseconds used for the recursion
var MILLI_SECONDS = 80; 
var totalMillis = 0;

// The interval before displaying a character from the entered string
var RESET_INTERVAL = 2000;
             
// =========================================
// Handles the initial loading of the canvas
// =========================================
function root_Loaded()
{
    // Provide access the object model
    activeXControl = document.getElementById("wpfeControl1");  
    
    // Allow the user to enter their text   
    var enterButton = document.getElementById("EnterButton");
    if (enterButton != null)
        enterButton.disabled = false;
        
    // make the cursor on the screen flash
    FlashCursor();
}

function FlashCursor()
{
    var cursor = activeXControl.FindName("CursorTextBlock");
    if (cursor != null)
    {
        if (cursor.opacity == 1)
            cursor.opacity = 0;
        else
            cursor.opacity = 1;
    }
    
    recurse0 = setTimeout("FlashCursor()", 500);
}

// ============================================
// Handles the user clicking the "Enter" button
// ============================================
function EnterButton_Click()
{
    // Reset the fields
    for (var i=1; i<=TOTAL_CHARACTERS; i++)
    {
        var element = activeXControl.FindName("Character" + i);
        element.Text = "";
        element.Opacity = 1;
        opacity = parseFloat("1.0");
    }

    // Begin to animate the entered text
    var textControl = document.getElementById("NameInput");
    if (textControl != null)
    {
        // First, hide the blinking cursor and remove the animation
        var cursor = activeXControl.FindName("CursorTextBlock");
        if (cursor != null)
        {
            clearTimeout(recurse0);
            cursor.Text = "";
        }
        
        // Store the value to be displayed
        var textToDisplay = new String();
        textToDisplay = textControl.value;        
        textValue = textToDisplay.split("");
        
        // Create the offset to center the text
        offset = Math.ceil((TOTAL_COLUMNS - textToDisplay.length) / 2)+1;
        
        // Set the flags determining if the column has been set
        columnSet = new Array();
        for (var i=0; i<textToDisplay.length; i++)
            columnSet.push(false);
        
        // Now begin to animate the name
        SetCharacters();
    }
}

function SetCharacters()
{
    // Determine if we are done animating the text
    var allTrue = true;
    for (var i=0; i<columnSet.length; i++)
        allTrue = allTrue && columnSet[i];
    if (allTrue)
    {
        clearTimeout(recurse1);
        HideRemainingCharacters();
        return;
    }
        
    // Increment the millis to determine if it is time to display a letter or not
    totalMillis = totalMillis + MILLI_SECONDS;
    
    // Determine if we need to display a letter
    if (totalMillis > RESET_INTERVAL)
    {
        RevealCharacter();
        totalMillis = 0;
    }

    for (var i=1; i<=TOTAL_CHARACTERS; i++)
    {
        var changeCharacter = true;
        
        // Determine if the column has been set
        var tempIndex = (i%TOTAL_COLUMNS);
        if ((tempIndex >= offset) && (tempIndex < (offset+columnSet.length)))
        {
            var columnIndex = tempIndex-offset;
            if (columnSet[columnIndex] == true)
                changeCharacter = false;
        }
        
        if (changeCharacter == true)
        {
            // First retrieve a character textblock
            var characterTextBlock = activeXControl.FindName("Character" + i);
            if (characterTextBlock != null)
            {        
                // Set the textblock to a random character from the character array
                var randomCharacterIndex = Math.floor(Math.random() * characters.length);
                characterTextBlock.text = characters[randomCharacterIndex];
            
                // Set the textblock to a random color from the color array
                var greenIndex = Math.floor(Math.random() * greens.length);
                characterTextBlock.foreground = greens[greenIndex];
            }
        }
    }
    
    // Recursively iterate until the name is completely displayed
    recurse1 = setTimeout("SetCharacters()", MILLI_SECONDS);
}

function RevealCharacter()
{
    // Grab a random character from the entered text        
    var randomIndex = Math.floor(Math.random() * textValue.length);        
    var randomCharacter = textValue[randomIndex];
    while (randomCharacter == null)
    {
        randomIndex = Math.floor(Math.random() * textValue.length);
        randomCharacter = textValue[randomIndex];
    }        
    textValue[randomIndex] = null;
           
    // Display the character on the 4th row of within the correct column    
    var characterNumber = offset + randomIndex + 60;
    var characterTextBlock = activeXControl.FindName("Character" + characterNumber);
    if (characterTextBlock != null)
    {
        // Set the character
        characterTextBlock.text = randomCharacter;
        columnSet[randomIndex] = true;
        
        // make the rest of the characters in the column blank
        for (var i=(offset+randomIndex); i<TOTAL_CHARACTERS; i+=TOTAL_COLUMNS)
        {
            if (i != characterNumber)
            {
                var columnEntry = activeXControl.FindName("Character"+i);
                if (columnEntry != null)
                    columnEntry.text = "";
            }
        }      
    }
}

function HideRemainingCharacters()
{    
    // Determine if we have reached the base case
    if (opacity == -.05)
    {
        clearTimeout(recurse2);
        return;
    }        

    for (var i=1; i<=TOTAL_CHARACTERS; i++)
    {
        var fadeCharacter = true;
        
        // Determine if the column has been set
        var tempIndex = (i%TOTAL_COLUMNS);
        if ((tempIndex >= offset) && (tempIndex < (offset+columnSet.length)))
        {
            var columnIndex = tempIndex-offset;
            if (columnSet[columnIndex] == true)
                fadeCharacter = false;
        }
        
        if (fadeCharacter == true)
        {
            // First retrieve a character textblock
            var characterTextBlock = activeXControl.FindName("Character" + i);
            if (characterTextBlock != null)
            {   
                characterTextBlock.Opacity = opacity;                 
            }
        }
    }   
    
    // Recursively iterate until the name is completely displayed
    opacity = opacity - .05;
    recurse2 = setTimeout("SetCharacters()", 100); 
}