'use strict';(function(window, $, Routing, swal) {    class AchievementsListApp {        constructor($wrapper, $achvsData, $achvsNames, $chalsNames, $spsNames)        {            this.$wrapper = $wrapper;            this.updateAchvsData = $achvsData;            this.userData = $achvsData['userdata'];            this.lockedAchvs = $achvsData['lockedachvs'];            this.achvsNames = $achvsNames;            this.chalsNames = $chalsNames;            this.spsNames = $spsNames;            this.displayedOption = 0;            let month = [];            month[0] = "January";            month[1] = "February";            month[2] = "March";            month[3] = "April";            month[4] = "May";            month[5] = "June";            month[6] = "July";            month[7] = "August";            month[8] = "September";            month[9] = "October";            month[10] = "November";            month[11] = "December";            this.month = month;            updateAchvs(this.updateAchvsData);            this.$wrapper.on(                'click',                '.js-delete-achievement',                this.handleAchievementDelete.bind(this)            );            this.$wrapper.on(                'click',                '.js-show-detail',                this.handleAchievementDetail.bind(this)            );        }        handleAchievementDetail(e){            e.preventDefault();            let $span = $(e.currentTarget)[0];            this.displayedOption = 0;            let id = $span.dataset.id;            let name = $span.dataset.name;            let type = $span.dataset.type;            let img = $span.dataset.img;            let date = $span.dataset.date.split('.');            let d = new Date();            if(this.lockedAchvs[id]){                swal({                    html:                        '<div class="col-sm-4">' +                            '<img src="' + img + '" class="img-circle" width="250" height="250" ' +                                'style="background-color: #EEEEEE" onerror="imageError(this)">' +                        '</div>' +                        '<div>' +                            '<div>' +                                '<h1 style="padding-top: 0pxs"><strong> - ' + name + ' - </strong></h1>' +                                '<h3 style="padding-top: 0pxs; color: ' + (type === "bronze" ? "brown" : type) + '">' +                                    '<i> (' + type + ')    </i></h3>' +                            '</div>' +                            '<div class="col-sm-8">' +                                '<span class="js-detail-left">' +                                    '<i class="btn fa fa-caret-left fa-5x col-sm-1 paddingless"></i>' +                        '       </span>' +                                '<div class="col-sm-9 js-detail-body-wrapper" style="text-align: left">' +                                    requirementsTemplate(this.lockedAchvs[id], this.userData) +                                '</div>' +                                '<span class="js-detail-right">' +                                    '<i class="btn fa fa-caret-right fa-5x col-sm-1 paddingless"></i>' +                                '</span>' +                            '</div>' +                        '</div>',                    width: '60%',                    showConfirmButton: false,                });                $('.js-detail-right').on('click', 'i', this.changeDetailBody.bind(this, +1, id));                $('.js-detail-left').on('click', 'i', this.changeDetailBody.bind(this, +4, id));            }            else{                swal({                    html:                    '<div class="col-sm-4 paddingless">' +                        '<img src="' + img + '" class="img-circle" width="250" height="250" ' +                            'style="background-color: #EEEEEE" onerror="imageError(this)">' +                    '</div>' +                    '<div class="col-sm-7">' +                        '<div>' +                            '<h1 style="padding-top: 0pxs"><strong> - ' + name + ' - </strong></h1>' +                            '<h3 style="padding-top: 0pxs; color: ' + (type === "bronze" ? "brown" : type) + '">' +                                '<i> (' + type + ')    </i></h3>' +                            '<h3 style="text-align: left">Completed on:</h3>' +                            '<h1>' + date[0] + '. ' + this.month[date[1] - 1] + '. ' + date[2] + '</h1>' +                        '</div>' +                    '</div>',                    width: '60%',                    showConfirmButton: false,                });                $('.swal2-container').attr("onclick", 'swal.closeModal();');            }            $('.swal2-header').remove();        }        static getComplete(req, data, composite = false){            if(req.length == 0)                return "-";            else{                let count = 0, num = 0;                for(let key in req){                    if(composite){                        if (data[key] === undefined)                            ++count;                    }                    else {                        if (data[key] === undefined || data[key] < req[key])                            ++count;                    }                    ++num;                }                return Math.floor(((num - count) / num) * 100) + "%";            }        }        changeDetailBody(delta, id){            this.displayedOption = (this.displayedOption + delta) % 5;            let html = "";            let $wrapper = $('.js-detail-body-wrapper');            switch (this.displayedOption){                case 0: html = requirementsTemplate(this.lockedAchvs[id], this.userData); break;                case 1: html = this.generateSpecsTemplate                                        (id, 'chals', 'chals', 'Challenges:', this.chalsNames);                                break;                case 2: html = this.generateSpecsTemplate                                        (id, 'sps', 'sps', 'Sports:', this.spsNames);                                break;                case 3: html = this.generateSpecsTemplate                                        (id, 'chalsps', 'chalsps', 'Challenges in Sports:', this.chalsNames, this.spsNames);                                break;                case 4: html = this.generateSpecsTemplate                                        (id, 'achvs', 'achvs', 'Achievements:', this.achvsNames);                                break;            }            let $element = $wrapper.find('.js-detail-body');            $element.fadeOut('250', () => {                $element.remove();                $wrapper.append(html);            });        }        generateSpecsTemplate(id, specs, usr, headline, allEntNames1, allEntNames2 = null){            let html = "";            if(this.lockedAchvs[id][specs].length !== 0) {                let reqs = this.lockedAchvs[id][specs];                for(let key in reqs){                    if(allEntNames2 !== null)                    {                        html += `<h4 class="col-sm-8">                                    ${ trunc(allEntNames1[key],20) } +                                     ${ trunc(allEntNames2[reqs[key]],20) }                                 </h4>                                 <h4 class="pull-right"                                      style="color:                                         ${ (this.userData[usr][key] !== undefined                                           && this.userData[usr][key][reqs[key]] !== undefined) ? 'green' : 'red' }">                                 <strong>                                     ${ (this.userData[usr][key] !== undefined                                         && this.userData[usr][key][reqs[key]] !== undefined) ? 1 : 0 } / 1                                 </strong></h4>`;                    }                    else                    {                        html += `   <h4 class="col-sm-8">${ trunc(allEntNames1[key],20) }</h4>                                    <h4 class="pull-right"                                         style="color:                                             ${( this.userData[usr][key] === undefined ? 0 :                                                             this.userData[usr][key] === null ? 1 :                                                                     this.userData[usr][key]) >= (                                                                        specs === 'achvs' ? "1" :                                                                             reqs[key]) ? "green" : "red"}" >                                    <strong>                                     ${ this.userData[usr][key] === undefined ? 0 :                                                 specs === 'achvs' ? 1 : this.userData[usr][key] }                                     /                                     ${ specs === 'achvs' ? "1" : reqs[key] }                                 </strong></h4>`;                    }                }            }            else                html = '<h4 class="col-sm-8" align="center" style="width: 100%"><i>empty</i></h4>';            return '<div class="js-detail-body">' +              '<h3>' + headline + '</h3>' +              '<div>' +                    html +              '</div>' +        '</div>';        }        handleAchievementDelete(e) {            e.preventDefault();            const $link = $(e.currentTarget);            const entity = "achievement";            const preConf = this._deleteAchievement.bind(null, $link);            deleteSwal(entity, preConf);        }        _deleteAchievement($link) {            $link.addClass('text-danger');            $link.find('.fa')                .removeClass('fa-trash')                .addClass('fa-spinner')                .addClass('fa-spin');            const deleteUrl = $link.data('url');            const $achievement = $link.closest('div');            return $.ajax({                url: deleteUrl,                method: 'POST'            }).then(() => {                    $achievement.fadeOut('normal');            })        }    }    const requirementsTemplate = (lockedAchvs, userData) =>{        let reqChal = AchievementsListApp.getComplete(lockedAchvs['chals'], userData['chals']);        let reqSp = AchievementsListApp.getComplete(lockedAchvs['sps'], userData['sps']);        let reqChalSp = AchievementsListApp.getComplete(lockedAchvs['chalsps'],userData['chalsps'], true);        let reqAchvs = AchievementsListApp.getComplete(lockedAchvs['achvs'],userData['achvs']);        return `            <div class="js-detail-body">                  <h3>Requirements:</h3>                  <div>                        <h4 class="col-sm-8">Challenges: </h4>                        <h4 class="pull-right" style="color: ${ reqChal === '-' ? 'black' :                                                                     reqChal === '100%' ? 'green' : 'red' }"><strong>                            ${ reqChal }                         </strong></h4>                        <h4 class="col-sm-8">Sports: </h4>                        <h4 class="pull-right" style="color: ${ reqSp === '-' ? 'black' :                                                                     reqSp === '100%' ? 'green' : 'red' }"><strong>                            ${ reqSp }                        </strong></h4>                        <h4 class="col-sm-8">Challenges in sports: </h4>                        <h4 class="pull-right" style="color: ${ reqChalSp === '-' ? 'black' :                                                                     reqChalSp === '100%' ? 'green' : 'red' }"><strong>                            ${ reqChalSp }                        </strong></h4>                        <h4 class="col-sm-8">Achievements: </h4>                        <h4 class="pull-right" style="color: ${ reqAchvs === '-' ? 'black' :                                                                     reqAchvs === '100%' ? 'green' : 'red' }"><strong>                            ${ reqAchvs}                        </strong></h4>                  </div>            </div>`    };    window.AchievementsListApp = AchievementsListApp;})(window, jQuery, Routing, swal);function trunc(str, count){    if(str.length > count) {        return str.substring(0,count-1)+"...";    }    return str;}