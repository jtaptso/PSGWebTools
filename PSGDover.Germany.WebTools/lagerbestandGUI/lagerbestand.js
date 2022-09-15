const toggleSwitch = document.querySelector('input[type="checkbox"]');
const toggleIcon = document.getElementById('toggle-icon');
const welcomeMsg = document.getElementById('welcomeMsg');
const itemcode = '';//document.getElementById('itemcode');
const txtareaItem = document.getElementById('txtAreaitemcode');
const bestaendeTable = document.getElementById('table');
const getLagerBtn = document.getElementById('getLagerBtn');
const clearBtn = document.getElementById('clearBtn');
let scanMode = true;
const LIGHT_THEME = 'light';
let tableclass  = '';
let errmsg = '';
let table = '';
let createdTable = '';
let createdheaders = '';
let createdtableBody = '';
let createdrows = '';
let switching = '';
let shouldSwitch = '';
let i, x, y;
let itArea = '';
let action = 'Lagerbestands';
let Lagerbestaende = [];
let headers = ['Lager', 'Nummer', 'Beschreibung'];
let connectData = {};
//let apiUrl = '';
// let price = 0;

async function getConnectionData(){
    const apiUrl = 'http://psg-ger-sap:8082/Connect';
    const apiUrlTst = `http://localhost:5234/Connect`;
    try {
        //const response = await fetch(apiUrlTst);
        //connectData = await response.json();
        welcomeMsg.textContent = 'Welcome on PSG Lagerbestand'; //`Welcome on ${connectData.companyName}`;
    } catch (error) {
        alert(error);
    }
}
async function getLagerbestaende(){
    itArea = txtareaItem.value;
    const apiUrlTest = `http://localhost:5000/SAPb1/Lagerbestands/${itArea}`;
    const apiUrl = `http://psg-ger-sap:8082/Lagerbestands?ItemCode=${itArea}`;
    const apiUrlTst = `http://localhost:5234/Lagerbestands?ItemCode=${itArea}`;
    try {
        if(itArea != ""){
            const response = await fetch(apiUrl);
            Lagerbestaende = await response.json();
            
            tableCreate(Lagerbestaende);
        }else {
            errmsg = 'Please make sure the itemcode really exists!';
            alert(errmsg);
        }
    } catch (error) {
        alert(error);
    }
}

function addItemNr() {
    if(txtareaItem.value != "" && scanMode)
        txtareaItem.value += ',';
}

function tableCreate(bestandArray) {

    tableclass = document.getElementsByClassName('table-content');
    while (document.getElementsByClassName('table-content')[0]) {
        document.getElementsByClassName('table-content')[0].remove();
    }
    table = document.createElement('table');

    table.classList.add('table-content');
    table.classList.add('table');
    table.setAttribute('id','sortMe');

    let headerRow = document.createElement('tr');
    for (let j = 0; j < headers.length; j++) {
        let header = document.createElement('th');
        header.setAttribute('onclick', `sortTable(${j})`);
        let textNode = document.createTextNode(headers[j]);
        header.appendChild(textNode);
        headerRow.appendChild(header);
    }

    table.appendChild(headerRow);

    for (let index = 0; index < bestandArray.length; index++) {
        const element = bestandArray[index];
        fillTable(table, element);
    }
  }

  function fillTable(table, item){
    let row = document.createElement('tr');
    for(key in item){
        let cell = document.createElement('td');
        let textNode = document.createTextNode(item[key]);
        cell.appendChild(textNode);
        row.appendChild(cell);
    }
    table.appendChild(row);
    bestaendeTable.appendChild(table);
  }

function update() {
    if(table)
        table.remove();
    txtareaItem.value = '';
}

// Switch Input Mode
function switchMode(event) {
    if (event.target.checked) {
        toggleIcon.children[0].textContent = 'Enter Mode';
        scanMode = false;
    }else{
        toggleIcon.children[0].textContent = 'Scan Mode';
        scanMode = true;
    }
}

function sortTable(n) {
    var table, rows, switching, i, x, y, shouldSwitch;
    table = document.getElementById("sortMe");
    switching = true;
    /* Make a loop that will continue until
    no switching has been done: */
    while (switching) {
      // Start by saying: no switching is done:
      switching = false;
      rows = table.rows;
      /* Loop through all table rows (except the
      first, which contains table headers): */
      for (i = 1; i < (rows.length - 1); i++) {
        // Start by saying there should be no switching:
        shouldSwitch = false;
        /* Get the two elements you want to compare,
        one from current row and one from the next: */
        x = rows[i].getElementsByTagName("TD")[n];
        y = rows[i + 1].getElementsByTagName("TD")[n];

        // Check if the two rows should switch place:
        if (x.innerHTML > y.innerHTML) {
          // If so, mark as a switch and break the loop:
          shouldSwitch = true;
          break;
        }
      }
      if (shouldSwitch) {
        /* If a switch has been marked, make the switch
        and mark that a switch has been done: */
        rows[i].parentNode.insertBefore(rows[i + 1], rows[i]);
        switching = true;
      }
    }
  }

//On Load
getConnectionData();

// Event Listener
toggleSwitch.addEventListener('change', switchMode);

txtareaItem.addEventListener('input', addItemNr); 
getLagerBtn.addEventListener('click', getLagerbestaende);
clearBtn.addEventListener('click', update);

