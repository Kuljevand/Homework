$(document).ready(function () {
    $('dropdown-select').select2({
        allowClear: true,
        theme: 'boostrap-5'

    });

    //#region Service

    let serviceInput = document.querySelector("#ServiceId");

    if (serviceInput) {
        let buildingType = document.querySelector(".BuildingTypeId");
        let buildingSize = document.querySelector(".BuildingSize");
        let color = document.querySelector(".Color");
        let noOfWindows = document.querySelector(".NoOfWindows");
        let noOfDoors = document.querySelector(".NoOfDoors");

        serviceInput.addEventListener("change", function () {

            buildingType.classList.add("d-none");
            buildingSize.classList.add("d-none");
            color.classList.add("d-none");
            noOfWindows.classList.add("d-none");
            noOfDoors.classList.add("d-none");

            let serviceId = serviceInput.value;

            if (serviceId == null) {
                return;
            }
            if (serviceId == 6) {
                buildingType.classList.remove("d-none");
                noOfWindows.classList.remove("d-none");
                noOfDoors.classList.remove("d-none");
            }
            if (serviceId == 7) {
                buildingType.classList.remove("d-none");
                buildingSize.classList.remove("d-none");
                color.classList.remove("d-none");
            }
            if (serviceId == 8) {
                buildingType.classList.remove("d-none");
                buildingSize.classList.remove("d-none");
            }
            if (serviceId == 9) {
                buildingType.classList.remove("d-none");
                buildingSize.classList.remove("d-none");
            };
        }
        )
    };

    //#endregion



    //#region Role

    let roleInput = document.querySelector("#Role");
    let dateOfEstInput = document.querySelector(".DateOfEstablishment");
    let noOfEmpInput = document.querySelector(".NumberOfEmployees");
    let activitiesInput = document.querySelector(".Activities");

    DateOfEstablishment.max = new Date().toISOString().split("T")[0];

    if (roleInput) {
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

                document.querySelector(".DateOfEstablishment").value = null;
                document.querySelector(".NumberOfEmployees").value = null;

            }

        })

    }

    //#endregion 

    





})