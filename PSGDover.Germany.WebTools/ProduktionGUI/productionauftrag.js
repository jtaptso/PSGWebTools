const tabProdauftrag = document.getElementById('prodAuftTab');
const txtIdNummer = document.getElementById('idNummer');
const welcomeMsg = document.getElementById('welcomeMsg');
const paElt = document.getElementById('pa');
const userElt = document.getElementById('user');
const panummerElt = document.getElementById('PaNr');
const clearIdBtn = document.getElementById('clearIdBtn');
const clearpaBtn =  document.getElementById('clearPaBtn');
const paStatus =  document.getElementById('paStatus');
const savePaBtn =  document.getElementById('savePaBtn');
const output = document.getElementById('output');
const logoutBtn = document.getElementById('logoutBtn');
const greetMsg = document.getElementById('greeting');
let selectedSize;
let statusCode;
let radioButtons = [];
let userName = {};
let produktionsAuftrag = {};
let previousStatus;
let pa_status = ["Nicht eingeplant", "Montage eingeplant", "Montage gestartet", "Montage unterbrochen", "Montage abgeschloßen", "Prüfstand abgeschloßen"];


function openTab(evt, cityName) {
    var i, tabcontent, tablinks;
    tabcontent = document.getElementsByClassName("tabcontent");
    for (i = 0; i < tabcontent.length; i++) {
      tabcontent[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
      tablinks[i].className = tablinks[i].className.replace(" active", "");
    }
    document.getElementById(cityName).style.display = "block";
    evt.currentTarget.className += " active";
}

//Get the nummer and send to the server "ConnectUser/{IDNummer}"
async function findTheName() {
  try {
    if (txtIdNummer.value) {
      const apiUrlTest = `http://localhost:5000/SAPb1/GetMonteur/${txtIdNummer.value}`;
      const apiUrl = `http://psg-ger-sap:8082/GetMonteur?IDNummer=${txtIdNummer.value}`;
      const newapiUrl = `http://localhost:5234/GetMonteur?IDNummer=${txtIdNummer.value}`;

      const response = await fetch(newapiUrl);
      userName = await response.json();
      console.log(userName);
      if (userName.userExists) {
        welcomeMsg.innerHTML = `<span style="color:blue;font-weight:bold;">Hallo, ${userName.name}! Bitte geben Sie eine PA-Nummer ein!</span>`;
        userElt.hidden = true;
        paElt.removeAttribute("hidden");
        panummerElt.focus();
        logoutBtn.hidden = false;
        savePaBtn.disabled = true;
        greetMsg.removeAttribute("hidden");
        greetMsg.innerHTML = `<span style="color:orange;font-size: 12px;">${userName.name} ist eingeloggt!</span>`
      }else{
        welcomeMsg.innerHTML = `<span style="color:red;font-weight:bold;">Sorry, Es gibt keinen User mit dieser Nummer.</span>`;
        
      }
    }
  } catch (error) {
    welcomeMsg.innerHTML = `<span style="color:red;font-weight:bold;">Bitte geben Sie einen gültigen Wert !</span>`;
  }
}

async function GetProduktionAuftrag() {
      if (panummerElt.value) {
        paStatus.innerHTML = '';
        //const apiUrlProdTest = `http://localhost:5000/SAPb1/GetProdAuftrag/${panummerElt.value}`;
        const apiUrlProd = `http://psg-ger-sap:8082/GetProdAuftrag?PANummer=${panummerElt.value}`;
        const newapiUrlProd = `http://localhost:5234/GetProdAuftrag?PANummer=${panummerElt.value}`;
        const response = await fetch(newapiUrlProd);
        produktionsAuftrag = await response.json();
        let past = Number(produktionsAuftrag.paStatus);
        previousStatus = past;
        let startIndex = 1;
        let endIndex = pa_status.length;
        savePaBtn.removeAttribute("hidden");
        if (userName.userTyp === 'M') {
          if (past >= 2 && past <= 5) {
            savePaBtn.disabled = false;
            endIndex = pa_status.length - 1;
            if (past > 2) {
              startIndex = 2;
            }
            if (past > 3 ) {
              startIndex = 3;
              if(past === 4){
                startIndex = 2;
              }
            }
            if (past > 4) {
              startIndex = 4;
            }
            if (past > 5) {
              startIndex = 5;
            }
            displayStatus(startIndex, endIndex, past);
          }else{
            welcomeMsg.innerHTML = `<span style="color:red;font-weight:red;">Dieser Auftrag steht noch nicht zur Verfügung.</span>`;
            savePaBtn.disabled = true;
            savePaBtn.style.opacity = 0.1;
          }
        } else {
          if (past >= 5) {
            startIndex = 4;
             if(past == 6){
              startIndex = 5;
            } 
            displayStatus(startIndex, endIndex, past);
          }else{
            welcomeMsg.innerHTML = `<span style="color:red;font-weight:bold;">Dieser Auftrag steht noch nicht zur Verfügung.</span>`;
            savePaBtn.disabled = true;
          }
        }
        radioButtons = document.querySelectorAll('input[name="status"]');
        if(radioButtons.length === 1){
          savePaBtn.disabled = true;
        }else{
          savePaBtn.disabled = false;
        }
      }
}

async function displayStatus(startindex, endIndex, selectedIndex) {
  for (let index = startindex; index < endIndex; index++) {
            var di = document.createElement("DIV");
            var inp = document.createElement("INPUT");
            var lab = document.createElement("LABEL");
            inp.setAttribute("type", "radio");
            inp.setAttribute("name", "status");
            inp.setAttribute("value", `${pa_status[index]}`);
            di.setAttribute("id", "spStat");
            savePaBtn.style.opacity = 1;
            var id = `${index+1}`;
            var lval = `<span style="font-weight:bold font-size: 10px;">${pa_status[index]}</span>`
            if(selectedIndex === Number(id)){
              inp.setAttribute("checked", true);
            }
            inp.setAttribute("id", `Status_${id}`);
            lab.setAttribute("FOR", `Status_${id}`);
            lab.innerHTML = lval;
            di.append(inp);
            di.append(lab);
            paStatus.append(di);
          }
          savePaBtn.disabled = false;
          welcomeMsg.innerHTML = `<span style="color:blue;font-weight:bold;">Gewählter Status:</span>`;
}

function logOut() {
  location.reload(); 
  greetMsg.hidden = true;
}

async function UpdateProduktionsAuftrag() {
  let statusCode;
  if (panummerElt.value) {
    output.innerHTML = '';
    for (const radioButton of radioButtons) {
      if (radioButton.checked) {
        var dfe = radioButton.id;
        selectedSize = radioButton.value;
        statusCode = radioButton.id.substring(7);
        produktionsAuftrag.paStatus = statusCode;
        produktionsAuftrag.docNum = panummerElt.value; 
        break;
      }
    }
    
    if (previousStatus === Number(statusCode)) {
      welcomeMsg.innerHTML = `<span style="color:blue;font-weight:bold;">Wählen Sie bitte einen anderen Status.</span>`;
    }else{
      const apiUrlProd = `http://psg-ger-sap:8082/UpdateProdAuftrag?DocNum=${panummerElt.value}&PaStatus=${statusCode}`;
      const newapiUrlProd = `http://localhost:5234/UpdateProdAuftrag?DocNum=${panummerElt.value}&PaStatus=${statusCode}&MonteurCode=${userName.code}`;
      const response = await fetch(newapiUrlProd);
      const pa = await response.json();
      if(pa.isUpdated){
        GetProduktionAuftrag();
        //output.innerHTML = `<span style="color:green;font-weight:lighter;font-size: 12px;">You selected : ${selectedSize}.</span>`;
      }
    }
    
  }
}

tabProdauftrag.addEventListener('click', () => {
  txtIdNummer.focus();
});

txtIdNummer.addEventListener('input', findTheName);
panummerElt.addEventListener('input', GetProduktionAuftrag);


clearpaBtn.addEventListener('click', () => {
	panummerElt.value = '';
  welcomeMsg.innerHTML = `<span style="color:blue;font-weight:bold;">Hallo, ${userName.name}! Bitte geben Sie eine PA-Nummer ein!</span>`;
  paStatus.innerHTML = '';
  savePaBtn.setAttribute('hidden', true);
  panummerElt.focus();
});
clearIdBtn.addEventListener('click', () => {
	txtIdNummer.value = '';

  txtIdNummer.focus();
});

savePaBtn.addEventListener('click', UpdateProduktionsAuftrag);
logoutBtn.addEventListener('click', logOut);
document.getElementById("prodAuftTab").click();