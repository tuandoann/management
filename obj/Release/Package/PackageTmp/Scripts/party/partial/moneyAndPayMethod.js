$(function () {
    //ini control
    init_controlInputMarskNumber("#moneyAndPayMethod input[name='pricePerTablePlan']");
    init_controlInputMarskNumber("#moneyAndPayMethod input[name='pricePerTableReal']");
    init_controlInputMarskNumber("#moneyAndPayMethod input[name='otherAmount']");
    init_controlInputMarskNumber("#moneyAndPayMethod input[name='depositAmount']");

    var isPayCashAfterDoneParty = document.querySelector('#isPayCashAfterDoneParty');
    var switchIsPayCashAfterDoneParty = new Switchery(isPayCashAfterDoneParty);

    var isPayBank = document.querySelector('#isPayBank');
    var switchIsPayBank = new Switchery(isPayBank);

    var isVAT = document.querySelector('#isVAT');
    var switchIsVAT = new Switchery(isVAT);
});