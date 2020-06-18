function Terminations(employees) {
    var count;
    for (each in employees){
        if (each.Status == false && each.End_Date.getFullYear() == Date.now.getFullYear()) {
            count++;
        }
    }
    location = document.getElementById("term_number");
    var num = document.createTextNode(count);
    location.innerHTML.appendChild(count);
    
}

function Calculate(employees) {
    var selection = document.getElementById("week_selection");
    var week = selection.value;

    for (each in employees) {
        count++;
    }

    var location = document.getElementById("count");
    location.innerHTML.appendChild(count);
}