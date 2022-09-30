const tabStueckliste = document.getElementById('stueckTab');
const subtitle = document.getElementById('stkMsg');
const txtItem = document.getElementById('itemcode');
const clearBtn = document.getElementById('clearBtn');
const loader = document.getElementById('loader');
let stkliste = document.querySelectorAll(".stk");
let togglers = document.querySelectorAll(".toggler"); 
//var togglers =  document.getElementsByClassName(".toggler");
const level1 = document.getElementById('level1');
const parentItem = document.getElementById('stkliste');
var liParent = document.getElementById('level1Parent');

let itCode = '';
let Oitem = [];

stkliste.forEach((stk) => {
    stk.addEventListener('click', () => {
        stk.classList.toggle("active");
    });
});

   /* togglers.forEach((toggler) => {
       toggler.addEventListener('click', () => {
           toggler.classList.toggle("active");
           toggler.nextElementSibling.classList.toggle("active");
       });
   }); */

async function getStueckliste() {
    // Get the itemcode entered in the textbox
    itCode = txtItem.value;
    parentItem.innerHTML = "";
    
    try {
        if(itCode != ""){
            const apiUrlTest = `http://localhost:5000/SAPb1/BOM/${itCode}`;
            const apiUrl = `http://psg-ger-sap:8082/BOM?itemcode=${itCode}`;
            const newUrl = `http://localhost:5234/BOM?itemcode=${itCode}`
            
            //parentItem.replaceChildren();
            parentItem.innerHTML = "";
            loader.hidden = false;
            const response = await fetch(newUrl);
            
            Oitem = await response.json();
            if(Oitem.father)
            {
                parentItem.innerHTML = "";
                //subtitle.innerText = 'Bitte geben Sie eine Stuecklistennummer ein.';
                var parent = document.createElement('Li');
                var divParent = document.createElement('div');
                divParent.className = 'toggler';
                divParent.innerHTML = `<span style="color:blue;font-weight:bold;">${Oitem.itemNummer}</span>`; //Oitem.itemNummer;
                parent.append(divParent);
                var ul_level1 = document.createElement('ul');
                ul_level1.className = "toggler-target";
                GetChildren(ul_level1, Oitem.children)

                loader.hidden = true;

                parent.append(ul_level1);
                parentItem.append(parent);

                txtItem.value = '';
                txtItem.focus();

                togglers = document.querySelectorAll(".toggler");
                stkliste = document.querySelectorAll(".stk");

                togglers.forEach((toggler) => {
                    toggler.addEventListener('click', () => {
                        toggler.classList.toggle("active");
                        toggler.nextElementSibling.classList.toggle("active");
                    });
                });

                stkliste.forEach((stk) => {
                    stk.addEventListener('click', () => {
                        stk.classList.toggle("active");
                    });
                });

                
            }
            else{
                subtitle.innerHTML = '<span style="color:red;font-weight:bold;font-size: 15px;">Item nicht gefunden !!.</span>'
                loader.hidden = true;
            } 
        }else {
            errmsg = 'Please make sure the itemcode really exists!';
            alert(errmsg);
        }
    } catch (error) {
        console.log(error);
    }
}

function GetChildren(ulParent, childrenList){
        childrenList.forEach(child => {
            var child_li = document.createElement('li');
            if (!child.hasChildren) {
                child_li.className = 'stk';
                child_li.innerHTML = `${child.itemNummer} | <span style="color:blue;font-weight:bold;">${child.quantity}</span> | <span style="color:green;font-weight:bold;">${child.lagerplatz}</span> | <span style="color:gray;font-size: 10px;">${child.itemName}</span> `;
            }else{
                var divElement = document.createElement('div');
                divElement.className = 'toggler';
                divElement.innerHTML = `${child.itemNummer} | <span style="color:blue;font-weight:bold;">${child.quantity}</span> | <span style="color:green;font-weight:bold;">${child.lagerplatz}</span> | <span style="color:gray;font-size: 10px;">${child.itemName}</span> `; //child.itemNummer;
                child_li.append(divElement);
                var child_ul = document.createElement('ul');
                child_ul.className = 'toggler-target';
                GetChildren(child_ul, child.children);
                child_li.append(child_ul);
            }
            ulParent.append(child_li);    
        });
}


//getStueckListeBtn.addEventListener('click', getStueckliste);
txtItem.addEventListener('change', getStueckliste);
clearBtn.addEventListener('click', () => {
	txtItem.value = '';
    parentItem.replaceChildren();
    txtItem.focus();
    subtitle.innerText = 'Bitte geben Sie eine Stuecklistennummer ein.';
});
tabStueckliste.addEventListener('click', () => {
    txtItem.focus();
});