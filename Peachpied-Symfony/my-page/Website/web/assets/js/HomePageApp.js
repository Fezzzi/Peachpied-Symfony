'use strict';

(function(window, $) {

    class HomePageApp {
        constructor($loginWrapper, $registerWrapper, $login) {
            this.$loginWrapper = $loginWrapper;
            this.$registerWrapper = $registerWrapper;
            this.activeLogin = $login;

            this.$loginWrapper.on(
                'click',
                '.js-headline',
                this.changeActiveElement.bind(this, true)
            );

            this.$registerWrapper.on(
                'click',
                '.js-headline',
                this.changeActiveElement.bind(this, false)
            )
        }

        changeActiveElement(login){
            if(login !== this.activeLogin){
                this.activeLogin = login;

                if(this.activeLogin === true)
                    this._activateElement(this.$loginWrapper, this.$registerWrapper);
                else
                    this._activateElement(this.$registerWrapper, this.$loginWrapper);
            }
        }

        _activateElement($toActivate, $toDeactivate){
            $toDeactivate.find('.js-headline')
                .removeClass('h2')
                .addClass('h3')
                .attr('style','color: Gray');

            $toActivate.find('.js-headline')
                .removeClass('h3')
                .addClass('h2')
                .attr('style','color: Black');

            $toDeactivate.find('.wrapper')
                .fadeOut('fast', () =>{
                    $toDeactivate
                        .attr('width', '33%');

                    $toActivate
                        .attr('width', '66%')
                        .find('.wrapper')
                        .fadeIn();
                });
        }
    }

    window.HomePageApp = HomePageApp;
}(window, jQuery));