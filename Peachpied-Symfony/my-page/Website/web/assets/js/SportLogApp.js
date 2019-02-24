'use strict';

(function(window, $, Routing, swal) {

    class SportLogApp {
        constructor($wrapper, $outer, $Challenges, $Sports, $updateAchvsData, $leaderboardData, $userId) {
            this.ROWS = 10;
            this.$form = $wrapper;
            this.$leaderboard = $('.leaderboard');
            this.$wrapper = $outer;
            this.openChallenges = $Challenges;
            this.openSports = $Sports;
            this.updateAchvsData = $updateAchvsData;
            this.leaderboardData = $leaderboardData;
            this.userId = $userId;
            this.leaderPage = 0;

            this.generateLeaderboardBody(this.leaderboardData);
            this.updatePoints(0);
            updateAchvs(this.updateAchvsData);
            this.setClickableRows();

            this.$wrapper.on(
                'click',
                '.js-delete-sport-log',
                this.handleSportLogDelete.bind(this)
            );
            this.$form.on(
                'submit',
                this.$form,
                this.handleNewFormSubmit.bind(this)
            );
            this.$wrapper.on(
                'click',
                '.js-show-image',
                this.handleImageShow.bind(this)
            );
            this.$leaderboard.on(
                'click',
                '.js-username',
                this.sortLeaderboard.bind(this,'username')
            );
            this.$leaderboard.on(
                'click',
                '.js-pts',
                this.sortLeaderboard.bind(this,'points')
            );
            this.$leaderboard.on(
                'click',
                '.js-logs',
                this.sortLeaderboard.bind(this,'challenges')
            );
            this.$leaderboard.on(
                'click',
                '.js-achvs',
                this.sortLeaderboard.bind(this,'achv_types')
            );

            this.$leaderboard.on(
                'click',
                '.js-page-left',
                this.pageLeft.bind(this)
            );

            this.$leaderboard.on(
                'click',
                '.js-page-right',
                this.pageRight.bind(this)
            )
        }

        pageLeft(){
            if(this.leaderPage > 0) {
                --this.leaderPage;
                $('.js-leaderboard-table-body')
                    .find('.js-leaderboard-row').remove();
                this.generateLeaderboardBody(this.leaderboardData);
            }
        }

        pageRight(){
            if((this.leaderPage + 1) * this.ROWS < this.leaderboardData.length) {
                ++this.leaderPage;
                $('.js-leaderboard-table-body')
                    .find('.js-leaderboard-row').remove();
                this.generateLeaderboardBody(this.leaderboardData);
            }
        }

        sortLeaderboard(key){
            let data = this.leaderboardData;

            function compare(a,b) {
                if (a[key].toLowerCase() < b[key].toLowerCase())
                    return -1;
                return 1;
            }

            function intcompare(a,b){
                if(parseInt(a[key]) > parseInt(b[key]))
                    return -1;
                return 1;
            }

            function achvcompare(a,b){
                if(9*a[key]['gold'] + 3*a[key]['silver'] + a[key]['bronze'] > 9*b[key]['gold'] + 3*b[key]['silver'] + b[key]['bronze'])
                    return -1;
                return 1;
            }

           data.sort(key === 'achv_types' ? achvcompare : (key === 'username' ? compare : intcompare));
            $('.js-leaderboard-table-body')
                .find('.js-leaderboard-row').remove();

            this.generateLeaderboardBody(data)
        }

        generateLeaderboardBody($data){
            const html = leaderboardTableBodyTemplate($data, this.leaderPage, this.$leaderboard, this.ROWS);
            const $template = $($.parseHTML(html));
            $('.js-leaderboard-table-body').append($template).hide().show('slow');
        }

        handleImageShow(e){
            let $span = $(e.currentTarget)[0];
            let img = '<img style="max-width: 100%; background-color: #EEEEEE" src="' + $span.dataset.img + '" ' +
                '           onerror="imageError(this)">';
            swal({
                html: img,
                width: 'auto',
                showConfirmButton: false,
            });

            $('.swal2-container').attr("onclick", 'swal.closeModal();');
        }
        
        setClickableRows(){
            this.$leaderboard.on(
                'click',
                '.clickable-row',
                (e) => {
                    window.location = $(e.currentTarget).data("href");
                }
            );
        }

        updatePoints(increment) {
            let $pointElements =  this.$wrapper.find('.js-points');
            let points = 0;
            let challenges = 0;
            let self = this;
            for (let element of $pointElements) {
                if(element.dataset.pts !== undefined) {
                    points += parseFloat(element.dataset.pts);
                    ++challenges;
                }
            }
            points = points.toFixed(2);
            for(let $user of this.leaderboardData)
                if($user.id === this.userId){
                    $user.points = points.toString();
                    $user.challenges = challenges.toString();
                }

            let levelData = this.getLevel(points);
            let $level = this.$wrapper.find('.js-level')[0];
            if($level.innerHTML !== levelData[1] && $level.innerHTML !== "")
                swal({
                    title: $level.innerHTML + "  >>  " + levelData[1],
                    width: 'auto',
                });

            let $elem = $('.js-level-block');

            $elem.find('.js-level-bar')[0].style = (levelData[0])[4];

            let bar = $elem.find('.js-level')[0];
            $(bar).hide();
            bar.innerHTML = levelData[1];
            $(bar).slideDown('medium');

            let pts = $elem.find('.js-total-points')[0];
            $(pts).hide();
            pts.innerHTML = points + levelData[2];
            $(pts).slideDown('medium');


            let $rows = $('.js-leaderboard-row').filter(function(){
                return $(this).data('id') === self.userId;
            });
            for(let $row of $rows){
                $($row).find('.js-user-points')[0].innerHTML = points + " pts";
                $($row).find('.js-user-challenges')[0].innerHTML = challenges;
            }

        }

        getLevel(points){
            let prelevel = null;
            for(let level in levels){
                if(levels[level] >= points){
                    let ptsNull = prelevel !== null ? levels[prelevel] : 0;
                    let ratio = (((points - ptsNull) / (levels[level] - ptsNull)) * 100) - 5;
                    let styleData0 = `left, #e00027, #e00027 ` + ratio + `%`;
                    let styleData1 = `,#FF3D49 ` + ratio + `%, #FF3D49 ` + (ratio + 10) + `%`;
                    let styleData2 = `,#FF6E7C ` + (ratio + 10) + `%, #FF6E7C ` + (ratio + 20) + `%`;
                    let styleData3 = `,#FFB8C1 ` + (ratio + 20) + `%, #FFB8C1 ` + (ratio + 30) + `%`;
                    let styleData4 = `,white ` + (ratio + 30) + `%, white ` + (ratio + 50) + `%`;
                    let style0 = SportLogApp.combineStyleData([styleData0]);
                    let style1 = SportLogApp.combineStyleData([styleData0, styleData1]);
                    let style2 = SportLogApp.combineStyleData([styleData0, styleData1, styleData2]);
                    let style3 = SportLogApp.combineStyleData([styleData0, styleData1, styleData2, styleData3]);
                    let style4 = SportLogApp.combineStyleData([styleData0, styleData1, styleData2, styleData3, styleData4]);
                    return [[style0, style1, style2, style3, style4], level, " / " + levels[level]];
                }
                prelevel = level;
            }
        }

        static combineStyleData(styleData){
            let concatData = "";
            for(let data in styleData) {
                concatData += styleData[data];
            }
            return `
                margin-left:25px; margin-right:25px; height: 150%;
                height: 10px;
                background:-moz-linear-gradient(`+ concatData + `);
                background:-webkit-linear-gradient(`+ concatData + `);
                background:-o-linear-gradient(`+ concatData + `);
                background:-linear-gradient(`+ concatData + `)`;
        }

        handleSportLogDelete(e) {
            e.preventDefault();

            const $link = $(e.currentTarget);
            const entity = "sport log";
            const preConf = this._deleteSportLog.bind(this, $link);

            deleteSwal(entity, preConf);
        }

        _deleteSportLog($link) {
            $link.addClass('text-danger');
            $link.find('.fa')
                .removeClass('fa-times')
                .addClass('fa-spinner')
                .addClass('fa-spin');

            const deleteUrl = $link.data('url');
            const $row = $link.closest('tr');
            let $self = this;

            return $.ajax({
                url: deleteUrl,
                method: "POST"
            }).then(() => {
                $row.fadeOut(() => {
                    $row.remove();

                    $self.updatePoints(-1)
                });
            })
        }

        handleNewFormSubmit(e) {
            e.preventDefault();

            const $form = $(e.currentTarget).serializeArray();
            const formData = {};
            formData['challenge'] = "";
            formData['sport'] = "";

            for (let fieldData of $form) {
                let splitted = fieldData.value.split(':');
                formData[fieldData.name + "Id"] = splitted[0];
                formData[fieldData.name] = splitted[1];
            }

            formData['image'] = document.getElementById("add-img").dataset.img;
            if(formData['image'] === "")
                formData['image'] = null;

            formData['points'] = this.openChallenges[formData['challengeId']] * this.openSports[formData['sportId']];
            console.log(this.openChallenges[formData['challengeId']], '*', this.openSports[formData['sportId']]);
            console.log(formData);

            this._saveSportLog(formData)
            .then((data) => {
                this._clearForm();
                formData['id'] = data['id'];
                for (let fieldData of $form) {
                    formData[fieldData.name] = fieldData.value.split(':')[1];
                }
                this._addRow(formData);
            }).catch((errorData) => {
                this._mapErrorsToForm(errorData.errors);
            });
        }

        _saveSportLog(data) {
            return new Promise((resolve, reject) => {
                const url = Routing.generate('sport_log_new');

                $.ajax({
                    url,
                    method: 'POST',
                    data: JSON.stringify(data)
                }).then((data, textStatus, jqXHR) => {
                    let id = jqXHR.getResponseHeader('Location').split('/')[2];
                    $.ajax({
                        url: jqXHR.getResponseHeader('Location')
                    }).then((data) => {
                        this._displaySuccess();
                        data['id'] = id;
                        resolve(data);
                    });
                }).catch((jqXHR) => {
                    const errorData = JSON.parse(jqXHR.responseText);
                    reject(errorData);
                });
            });
        }

        _displaySuccess(){
            const html = successTemplate();
            const $row = $($.parseHTML(html));

            this.$form.attr("hidden", true);
            this.$wrapper.find('.logs-form').hide().append($row).fadeIn();
            setTimeout(() =>{
                this.$wrapper.find('.log-success').fadeOut(() => {
                    this.$wrapper.find('.log-success').remove();
                    this.$form.attr("hidden", false);
                });
            },2000);
        }

        _mapErrorsToForm(errorData) {
            console.log(errorData);
            this._removeFormErrors();

            for (let element of this.$form.find(':input')) {
                const fieldName = $(element).attr('name');
                const $wrapper = $(element).closest('.form-group');
                if (errorData[fieldName]) {
                    const $error = $('<span class="js-field-error help-block"></span>');
                    $error.html(errorData[fieldName]);
                    $wrapper.append($error);
                    $wrapper.addClass('has-error');
                }
            }
        }

        _removeFormErrors() {
            this.$form.find('.js-field-error').remove();
            this.$form.find('.form-group').removeClass('has-error');
        }

        _clearForm() {
            this._removeFormErrors();
            this.$form[0].reset();
        }

        _addRow(sportLog) {
            const html = rowTemplate(sportLog, new Date(Date.now()));
            const $row = $($.parseHTML(html));

            this.$wrapper.find('.sport-list').prepend($row);

            this.updatePoints(1);
            resetIMG();
        }
    }

    const rowTemplate = (sportLog, date) => `
        <tr>
            <td class="col-md-3"><strong> ${ sportLog.challenge }</strong></td>
            <td>${ sportLog.sport }</td>
            <td class="js-points"
                data-pts="${ sportLog.points }">
                + ${ sportLog.points } pts</td>
            <td style="text-align: center">` +
                (date.getDate() > 9 ? "" : "0") + date.getDate() + '. ' + (date.getMonth() > 9 ? "" : "0") + (date.getMonth() + 1) + '. ' + date.getFullYear() +
            `</td>
            <td style="text-align: center">
            ${ (sportLog => {
                        if (sportLog.image === null) 
                            return ``;
                        else {
                            return `<span class = "btn full-div-button js-show-image" data-img = "${ sportLog.image }">SHOW</span>`}
                        })(sportLog)}
            </td>
            <td style="text-align: right">
                <a href="#"
                   class="btn js-delete-sport-log btn-min"
                   data-url="${ Routing.generate('sport_log_delete', { 'id': sportLog.id }) }"
                >
                    <span class="fa fa-times" style="color: #E00027"></span>
                </a>
            </td>
        </tr>
    `;


    const successTemplate = () => `
        <div class="log-success">
            <h3 class="alert alert-success">
                <p><span class="fa fa-thumbs-up"></span><strong> Success!</strong>
            </h3>
        </div>
    `;

    const leaderboardTableBodyTemplate = ($data, page, $leaderboard, ROWS) => {
        let string = '';
        if(page > 0)
            $leaderboard.find('.leaderboard-table-hover').removeClass('leaderboard-table-leaders');
        else
            $leaderboard.find('.leaderboard-table-hover').addClass('leaderboard-table-leaders');
        for (let i = 0; i < ROWS && (page * ROWS) + i < $data.length; ++i){
            let leader = $data[(page * ROWS) + i];
            leader.points = parseFloat(leader.points).toFixed(1);
            string +=
            '<tr class="js-leaderboard-row clickable-row" data-id="' + leader.id + '" data-href="' +
            Routing.generate('profile_visit', {'id': leader.id}) + '">' +
            '<td class="text-left">' + '<strong>' + leader.username + ' </strong>' +
            '</td>' +
            '<td class="text-center js-user-points">' + leader.points + ' pts</td>' +
            '<td class="text-center js-user-challenges" width="15%">' + leader.challenges + '</td>' +
            '<td width="35%">' + innerData(leader.achv_types) + '</td></tr>'
        }
        let $page = $('.js-page');
        $page[0].innerHTML = page + 1;
        if($data.length > ROWS)
            $page.show();
        else
            $page.hide();
        if(page > 0)
            $('.js-page-left').show();
        else
            $('.js-page-left').hide();

        if((page + 1) * ROWS < $data.length)
            $('.js-page-right').show();
        else
            $('.js-page-right').hide();
        return string;
    };

    const innerData = ($data) => {
        let innerstring ='';
        for (let key in $data) {
            innerstring +=
                ' <img class="img-rounded" style="background-color: #EEEEEE; margin-right: 3px" width="25px" height="25px" ' +
                '       src="/assets/images/achievement' + key + '.png" onerror="imageError(this)">' + $data[key]
        }
        return innerstring;
    };

    window.SportLogApp = SportLogApp;

})(window, jQuery, Routing, swal);
