'use strict';

function updateAchvs(data) {
    awardAchvs(data['userdata'], data['lockedachvs'])
}

function awardAchvs(userData, achvsReqs){
    let alerts = [];
    for(let key in achvsReqs){
        if(testReq(userData, achvsReqs[key])) {
            logAchievement(key);
            alerts.push(achvsReqs[key]);
        }
    }
    if(alerts.length > 0)
        displayAlert(alerts,0);
}

function testReq(data, reqs){
    for(let reqKey in reqs){
        if(reqKey !== "type" && reqKey !== "name") {
            if (reqs[reqKey] !== [])
                for (let innerKey in reqs[reqKey]) {
                    if (reqKey == 'achvs') {
                        if (!(innerKey in data[reqKey]))
                            return false
                    }
                    else if (data[reqKey] === undefined || data[reqKey][innerKey] === undefined || data[reqKey][innerKey] < reqs[reqKey][innerKey]) {
                        return false;
                    }
                }
        }
    }
    return true;
}

function logAchievement(achv_id){
    let formData = {};
    formData['achv_id'] = parseInt(achv_id);
    let d = new Date();
    formData['date'] = d.getDate() + '.' + (d.getMonth() + 1) + '.' + d.getFullYear();
    console.log(formData);

    return new Promise((resolve, reject) => {
        const url = Routing.generate('achievementLog_new');
        $.ajax({
            url,
            method: 'POST',
            data: JSON.stringify(formData)
        }).then((data, textStatus, jqXHR) => {
            $.ajax({
                url: jqXHR.getResponseHeader('Location')
            }).then((data) => {
                resolve(data);
            });
        }).catch((jqXHR) => {
            reject(errorData);
        });
    });

}

function displayAlert(alerts, index){
    let type = alerts[index]['type'];
    let name = alerts[index]['name'];
    swal({
        position: 'top-end',
        showConfirmButton: false,
        html: ` <div style="width: 100%">
                    <div class="col-md-3 paddingless" style="margin-top: 5px; margin-left: 5px">
                        <img src="assets/images/achievement` + type + `.png"  onerror="imageError(this)"
                            class="img-circle" style="background-color: #EEEEEE" width="50" height="50">
                    </div>
                    <div class="col-md-8">
                        <h4 class="paddingless">` + name + `<br>unlocked!</h4>
                    </div>
                </div>`,
        timer: 2500,
        padding: 1,
        toast: true,
    }).then(() => {
        ++index;
        if(index < alerts.length){
            displayAlert(alerts, index);
        }
    });
}