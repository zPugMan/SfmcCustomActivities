//create connection for the JourneyBuilder session
const connection = new Postmonger.Session();

//activity store
let activity = null;

document.addEventListener('DOMContentLoaded', function main() {
    ////test harness
    //setupTestHarness();

    //UI event handlers
    setupEventHandlers();

    //bind initActivity event
    connection.on('initActivity', onInitActivity);
    connection.on('requestedInteractionDefaults', requestedInteractionDefaults);
    connection.on('requestedInteraction', requestedInteraction);

    //tell JourneyBuilder ready to receive activity payload
    connection.trigger('ready');

    //tell JourneyBuilder want interaction defaults
    connection.trigger('requestedInteractionDefaults');

    //tell JourneyBuilder want interaction
    connection.trigger('requestedInteraction');
});

function onInitActivity(payload){
    activity = payload;

    const hasArgs = Boolean(
        activity.arguments &&
        activity.arguments.execute &&
        activity.arguments.execute.inArguments &&
        activity.arguments.execute.inArguments.length > 0
    );

    const inArgs = hasArgs ? activity.arguments.execute.inArguments : [];

    console.log('-------- triggered:onInitActivity({obj}) --------');
    console.log('activity:\n ', JSON.stringify(activity, null, 4));
    console.log('Has In Arguments: ', hasArgs);
    console.log('inArguments: ', inArgs);
    console.log('-------------------------------------------------');

    const smsKeywordArg = inArgs.find((arg)=> arg.smsKeyword);
    if (smsKeywordArg) {
        setInputTextElement('sms-keyword-id', smsKeywordArg.smsKeyword);
    }

    const contactPhoneArg = inArgs.find((arg)=>arg.smsPhone);
    if(contactPhoneArg) {
        setInputTextElement('contact-phone', contactPhoneArg.smsPhone);
    }

    const smsMessage = inArgs.find((arg)=>arg.smsMessage);
    if(smsMessage) {
        setInputTextElement('sms-message-txt', smsMessage.smsMessage);
    }

    onInputValueChange();
}

function setupEventHandlers() {
    //change events from form
    document.getElementById('sms-keyword-id').addEventListener('change', onInputValueChange);
    document.getElementById('sms-message-txt').addEventListener('change', onInputValueChange);
    document.getElementById('contact-phone').addEventListener('change', onInputValueChange);

    //button events
    document.getElementById('done').addEventListener('click', onDoneButtonClick);
    document.getElementById('cancel').addEventListener('click', onCancelButtonClick);
}

function onInputValueChange() {
    let hasMinimum;

    hasMinimum = Boolean(
        document.getElementById('sms-keyword-id').value &&
        document.getElementById('contact-phone').value &&
        document.getElementById('sms-message-txt').value
    );

    if (hasMinimum) {
        document.getElementById('done').removeAttribute('disabled');
    } else {
        document.getElementById('done').setAttribute('disabled','');
    }

    connection.trigger('setActivityDirtyState', true);
}

function onCancelButtonClick() {
    // tell Journey Builder that this activity has no changes.
    connection.trigger('setActivityDirtyState', false);

    // now request that Journey Builder closes the inspector/drawer
    connection.trigger('requestInspectorClose');
}

function onDoneButtonClick() {
    // we set must metaData.isConfigured in order to tell JB that
    // this activity is ready for activation
    activity.metaData.isConfigured = true;

    // get the option that the user selected and save it to
    activity.arguments.execute.inArguments = [{
        smsKeyword: document.getElementById('sms-keyword-id').value,
        smsPhone: document.getElementById('contact-phone').value,
        smsMessage: document.getElementById('sms-message-txt').value
    }];

    // you can set the name that appears below the activity with the name property
    activity.name = `SMS ${activity.arguments.execute.inArguments[0].smsKeyword} keyword`;

    console.log('------------ triggering:updateActivity({obj}) ----------------');
    console.log('Sending message back to updateActivity');
    console.log('saving\n', JSON.stringify(activity, null, 4));
    console.log('--------------------------------------------------------------');

    connection.trigger('updateActivity', activity);
}

function requestedInteractionDefaults(payload) {
    console.log('-------- requestedInteractionDefaults --------');
    console.log('payload\n', JSON.stringify(payload, null, 4));
    console.log('requestInteraction', payload);
    console.log('---------------------------------------------');
}

function requestedInteraction(payload) {
    console.log('-------- requestedInteraction --------');
    console.log('payload\n', JSON.stringify(payload, null, 4));
    console.log('requestInteraction', payload);
    console.log('--------------------------------------');

    //TODO refactor this to a hashmap or something; this is repetitive!
    if(activity && activity.arguments && activity.arguments.execute.inArguments.length > 0) {
        setInputTextElement('sms-keyword-id', activity.arguments.execute.inArguments[0].smsKeyword);
        setInputTextElement('contact-phone', activity.arguments.execute.inArguments[0].smsPhone);
        setInputTextElement('sms-message-txt', activity.arguments.execute.inArguments[0].smsMessage);
    }

    //inform journey builder of activity changes
    connection.trigger('setActivityDirtyState', true);
}

function setInputTextElement(elementId, textValue) {
    if(textValue){
        document.getElementById(elementId).value=textValue;
    }
}

//setup postmonger for local dev effort
//call `jb.ready()` from the console to kick off the initActivity event with a mock activity object
function setupTestHarness() {
    const isLocalhost = location.hostname === 'localhost' || location.hostname === '127.0.0.1';
    if (!isLocalhost) {
        // don't load the test harness functions when running in Journey Builder
        return;
    }

    const jbSession = new Postmonger.Session();
    const jb = {};
    window.jb = jb;

    jbSession.on('setActivityDirtyState', function(value) {
        console.log('[echo] setActivityDirtyState -> ', value);
    });

    jbSession.on('requestInspectorClose', function() {
        console.log('[echo] requestInspectorClose');
    });

    jbSession.on('updateActivity', function(activity) {
        console.log('[echo] updateActivity -> ', JSON.stringify(activity, null, 4));
    });

    jbSession.on('ready', function() {
        console.log('[echo] ready');
        console.log('\tuse jb.ready() from the console to initialize your activity')
    });

    // fire the ready signal with an example activity
    jb.ready = function() {
        jbSession.trigger('initActivity', {
            name: '',
            key: 'EXAMPLE-1',
            metaData: {},
            configurationArguments: {},
            arguments: {
                executionMode: "{{Context.ExecutionMode}}",
                definitionId: "{{Context.DefinitionId}}",
                activityId: "{{Activity.Id}}",
                contactKey: "{{Context.ContactKey}}",
                execute: {
                    inArguments: [
                        {
                            smsPhone: "(555) 111-2233",
                        },
                        {
                            smsKeyword: "DONATE2"
                        },
                        {
                            smsMessage: "Give 2 {{Contact.FirstName}} and we get 4!"
                        }
                    ],
                    outArguments: []
                },
                startActivityKey: "{{Context.StartActivityKey}}",
                definitionInstanceId: "{{Context.DefinitionInstanceId}}",
                requestObjectId: "{{Context.RequestObjectId}}"
            }
        });
    };
}