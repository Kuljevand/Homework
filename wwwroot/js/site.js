$(document).ready(function () {
    $('.dropdown-select').select2(); ({
        allowClear: true,
        theme: 'bootstrap-5'
    });


    let roleInput = document.querySelector("#Role");
    let dateOfEstInput = document.querySelector(".DateOfEstablishment");
    let noOfEmpInput = document.querySelector(".NumberOfEmployees"); 
    let activitiesInput = document.querySelector(".Activities");



    roleInput.addEventListener("change", function () {

        if (roleInput.value == "Изведувач") {
            noOfEmpInput.classList.remove("d-none")
            dateOfEstInput.classList.remove("d-none")
            activitiesInput.classList.remove("d-none")
        }
        else {
            noOfEmpInput.classList.add("d-none")
            dateOfEstInput.classList.add("d-none")
            activitiesInput.classList.add("d-none")

            document.querySelector(".DateOfEstablishment").value = Date.now; /*not working*/
            document.querySelector(".NumberOfEmployees").value = null;

        }

    })

});

