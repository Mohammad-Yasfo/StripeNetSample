﻿using Stripe;

namespace StripeNetSample.Infrastructure.Payment.Stripe;

/// <summary>
/// Handles Stripe exceptions by providing user-friendly error messages based on Stripe error codes.
/// </summary>
public class StripeExceptionHandler
{
    /// <summary>
    /// Retrieves a user-friendly error message based on the Stripe exception.
    /// </summary>
    /// <param name="ex">The Stripe exception instance.</param>
    /// <returns>A user-friendly error message string.</returns>
    public static string GetMessage(StripeException ex)
    {
        if (ex == null || ex.StripeError == null || ex.StripeError.Code == null)
            return "Not Available";

        string errorCode = ex.StripeError.Code;
        return errorCode switch
        {
            "account_already_exists" => "The email address provided for the creation of a this account already has an account associated with it. Try to login the existing account.",
            "account_country_invalid_address" => "The country of the business address provided does not match the country of the account. Businesses must be located in the same country as the account.",
            "account_invalid" => "The account ID provided as a value for the Stripe-Account header is invalid. Check that your requests are specifying a valid account ID.",
            "account_number_invalid" => "The bank account number provided is invalid (e.g., missing digits).",
            "amount_too_large" => "The specified amount is greater than the maximum amount allowed. Use a lower amount and try again.",
            "amount_too_small" => "The specified amount is less than the minimum amount allowed. Use a higher amount and try again.",
            "api_key_expired" => "Technical issue on our server, please report this issue.",
            "balance_insufficient" => "The transfer or payout could not be completed because your account does not have a sufficient balance available. Create a new transfer or payout using an amount less than or equal to the account’s available balance.",
            "bank_account_exists" => "The bank account provided already exists on the specified Customer object. If the bank account should also be attached to a different customer, include the correct customer ID when making the request again.",
            "bank_account_unusable" => "The bank account provided cannot be used for payouts. A different bank account must be used.",
            "bank_account_unverified" => "Your Connect platform is attempting to share an unverified bank account with a connected account.",
            "card_declined" => "The card has been declined. " + GetDeclineMessage(ex.StripeError.DeclineCode),
            "charge_already_captured" => "The charge you’re attempting to capture has already been captured. Update the request with an uncaptured charge ID.",
            "charge_already_refunded" => "The charge you’re attempting to refund has already been refunded. Update the request to use the ID of a charge that has not been refunded.",
            "charge_disputed" => "The charge you’re attempting to refund has been charged back. Check the disputes documentation to learn how to respond to the dispute.",
            "charge_exceeds_source_limit" => "This charge would cause you to exceed your rolling-window processing limit for this source type. Please retry the charge later, or contact us to request a higher processing limit.",
            "charge_expired_for_capture" => "The charge cannot be captured as the authorization has expired. Auth and capture charges must be captured within seven days.",
            "country_unsupported" => "You attempted to create a custom account in a country that is not yet supported. ",
            "customer_max_subscriptions" => "The maximum number of subscriptions for a customer has been reached. Contact us if you are receiving this error.",
            "email_invalid" => "The email address is invalid (e.g., not properly formatted). Check that the email address is properly formatted and only includes allowed characters.",
            "expired_card" => "The card has expired. Check the expiration date or use a different card.",
            "idempotency_key_in_use" => "The idempotency key provided is currently being used in another request. This occurs if your integration is making duplicate requests simultaneously.",
            "incorrect_address" => "The card’s address is incorrect. Check the card’s address or use a different card.",
            "incorrect_cvc" => "The card’s security code is incorrect. Check the card’s security code or use a different card.",
            "incorrect_number" => "The card number is incorrect. Check the card’s number or use a different card.",
            "incorrect_zip" => "The card’s postal code is incorrect. Check the card’s postal code or use a different card.",
            "instant_payouts_unsupported" => "This card is not eligible for Instant Payouts. Try a debit card from a supported bank.",
            "invalid_card_type" => "The card provided as an external account is not supported for payouts. Provide a non-prepaid debit card instead.",
            "invalid_charge_amount" => "The specified amount is invalid. The charge amount must be a positive integer in the smallest currency unit, and not exceed the minimum or maximum amount.",
            "invalid_cvc" => "The card’s security code is invalid. Check the card’s security code or use a different card.",
            "invalid_expiry_month" => "The card’s expiration month is incorrect. Check the expiration date or use a different card.",
            "invalid_expiry_year" => "The card’s expiration year is incorrect. Check the expiration date or use a different card.",
            "invalid_number" => "The card number is invalid. Check the card details or use a different card.",
            "invalid_source_usage" => "The source cannot be used because it is not in the correct state (e.g., a charge request is trying to use a source with a pending, failed, or consumed source). Check the status of the source you are attempting to use.",
            "invoice_no_customer_line_items" => "An invoice cannot be generated for the specified customer as there are no pending invoice items. Check that the correct customer is being specified or create any necessary invoice items first.",
            "invoice_no_subscription_line_items" => "An invoice cannot be generated for the specified subscription as there are no pending invoice items. Check that the correct subscription is being specified or create any necessary invoice items first.",
            "invoice_not_editable" => "The specified invoice can no longer be edited. Instead, consider creating additional invoice items that will be applied to the next invoice. You can either manually generate the next invoice or wait for it to be automatically generated at the end of the billing cycle.",
            "invoice_payment_intent_requires_action" => "This payment requires additional user action before it can be completed successfully. Payment can be completed using the PaymentIntent associated with the invoice.",
            "invoice_upcoming_none" => "There is no upcoming invoice on the specified customer to preview. Only customers with active subscriptions or pending invoice items have invoices that can be previewed.",
            "livemode_mismatch" => "Test and live mode API keys, requests, and objects are only available within the mode they are in.",
            "missing" => "Both a customer and source ID have been provided, but the source has not been saved to the customer. To create a charge for a customer with a specified source, you must first save the card details.",
            "not_allowed_on_standard_account" => "Transfers and payouts on behalf of a Standard connected account are not allowed.",
            "order_creation_failed" => "The order could not be created. Check the order details and then try again.",
            "order_required_settings" => "The order could not be processed as it is missing required information. Check the information provided and try again.",
            "order_status_invalid" => "The order cannot be updated because the status provided is either invalid or does not follow the order lifecycle (e.g., an order cannot transition from created to fulfilled without first transitioning to paid).",
            "order_upstream_timeout" => "The request timed out. Try again later.",
            "out_of_inventory" => "The SKU is out of stock. If more stock is available, update the SKU’s inventory quantity and try again.",
            "parameter_invalid_empty" => "One or more required values were not provided. Make sure requests include all required parameters.",
            "parameter_invalid_integer" => "One or more of the parameters requires an integer, but the values provided were a different type. Make sure that only supported values are provided for each attribute.",
            "parameter_invalid_string_blank" => "One or more values provided only included whitespace. Check the values in your request and update any that contain only whitespace.",
            "parameter_invalid_string_empty" => "One or more required string values is empty. Make sure that string values contain at least one character.",
            "parameter_missing" => "One or more required values are missing.",
            "parameter_unknown" => "The request contains one or more unexpected parameters. Remove these and try again.",
            "parameters_exclusive" => "Two or more mutually exclusive parameters were provided.",
            "payment_intent_authentication_failure" => "The provided payment method has failed authentication. Provide a new payment method to attempt to fulfill this PaymentIntent again.",
            "payment_intent_incompatible_payment_method" => "The PaymentIntent expected a payment method with different properties than what was provided.",
            "payment_intent_invalid_parameter" => "One or more provided parameters was not allowed for the given operation on the PaymentIntent.",
            "payment_intent_payment_attempt_failed" => "The latest payment attempt for the PaymentIntent has failed.",
            "payment_intent_unexpected_state" => "The PaymentIntent’s state was incompatible with the operation you were trying to perform.",
            "payment_method_unactivated" => "The charge cannot be created as the payment method used has not been activated. Activate the payment method in the Dashboard, then try again.",
            "payment_method_unexpected_state" => "The provided payment method’s state was incompatible with the operation you were trying to perform. Confirm that the payment method is in an allowed state for the given operation before attempting to perform it.",
            "payouts_not_allowed" => "Payouts have been disabled on the connected account. Check the connected account’s status to see if any additional information needs to be provided, or if payouts have been disabled for another reason.",
            "platform_api_key_expired" => "The API key provided by your Connect platform has expired. This occurs if your platform has either generated a new key or the connected account has been disconnected from the platform. Obtain your current API keys from the Dashboard and update your integration, or reach out to the user and reconnect the account.",
            "postal_code_invalid" => "The postal code provided was incorrect.",
            "processing_error" => "An error occurred while processing the card. Check the card details are correct or use a different card.",
            "product_inactive" => "The product this SKU belongs to is no longer available for purchase.",
            "rate_limit" => "Too many requests hit the API too quickly. We recommend an exponential backoff of your requests.",
            "resource_already_exists" => "A resource with a user-specified ID (e.g., plan or coupon) already exists. Use a different, unique value for id and try again.",
            "resource_missing" => "The ID provided is not valid. Either the resource does not exist, or an ID for a different resource has been provided.",
            "routing_number_invalid" => "The bank routing number provided is invalid.",
            "secret_key_required" => "The API key provided is a publishable key, but a secret key is required. Obtain your current API keys from the Dashboard and update your integration to use them.",
            "sepa_unsupported_account" => "Your account does not support SEPA payments.",
            "setup_attempt_failed" => "The latest setup attempt for the SetupIntent has failed.",
            "setup_intent_authentication_failure" => "The provided payment method has failed authentication. Provide a new payment method to attempt to fulfill this SetupIntent again.",
            "setup_intent_unexpected_state" => "The SetupIntent’s state was incompatible with the operation you were trying to perform.",
            "shipping_calculation_failed" => "Shipping calculation failed as the information provided was either incorrect or could not be verified.",
            "sku_inactive" => "The SKU is inactive and no longer available for purchase. Use a different SKU, or make the current SKU active again.",
            "state_unsupported" => "Occurs when providing the legal_entity information for a U.S. custom account, if the provided state is not supported. (This is mostly associated states and territories.)",
            "tax_id_invalid" => "The tax ID number provided is invalid (e.g., missing digits). Tax ID information varies from country to country, but must be at least nine digits.",
            "taxes_calculation_failed" => "Tax calculation for the order failed.",
            "testmode_charges_only" => "Your account has not been activated and can only make test charges. Activate your account in the Dashboard to begin processing live charges.",
            "tls_version_unsupported" => "Your integration is using an older version of TLS that is unsupported. You must be using TLS 1.2 or above.",
            "token_already_used" => "The token provided has already been used. You must create a new token before you can retry this request.",
            "token_in_use" => "The token provided is currently being used in another request. This occurs if your integration is making duplicate requests simultaneously.",
            "transfers_not_allowed" => "The requested transfer cannot be created. Contact us if you are receiving this error.",
            "upstream_order_creation_failed" => "The order could not be created. Check the order details and then try again.",
            "url_invalid" => "The URL provided is invalid.",
            _ => "Uknown error happened, please contact us.",
        };
    }

    /// <summary>
    /// Retrieves a user-friendly decline message based on the Stripe decline code.
    /// </summary>
    /// <param name="declineCode">The Stripe decline code.</param>
    /// <returns>A user-friendly decline message string.</returns>
    public static string GetDeclineMessage(string declineCode)
    {
        if (declineCode == null)
            return "Not Available";

        return declineCode switch
        {
            "authentication_required" => "The card was declined as the transaction requires authentication.",
            "approve_with_id" => "The payment cannot be authorized.",
            "call_issuer" => "The card has been declined for an unknown reason.",
            "card_not_supported" => "The card does not support this type of purchase.",
            "card_velocity_exceeded" => "The customer has exceeded the balance or credit limit available on their card.",
            "currency_not_supported" => "The card does not support the specified currency.",
            "do_not_honor" => "The card has been declined for an unknown reason.",
            "do_not_try_again" => "The card has been declined for an unknown reason.",
            "duplicate_transaction" => "A transaction with identical amount and credit card information was submitted very recently.",
            "expired_card" => "The card has expired.",
            "fraudulent" => "The payment has been declined as Stripe suspects it is fraudulent.",
            "generic_decline" => "The card has been declined for an unknown reason.",
            "incorrect_number" => "The card number is incorrect.",
            "incorrect_cvc" => "The CVC number is incorrect.",
            "incorrect_pin" => "The PIN entered is incorrect. This decline code only applies to payments made with a card reader.",
            "incorrect_zip" => "The ZIP/postal code is incorrect.",
            "insufficient_funds" => "The card has insufficient funds to complete the purchase.",
            "invalid_account" => "The card, or account the card is connected to, is invalid.",
            "invalid_amount" => "The payment amount is invalid, or exceeds the amount that is allowed.",
            "invalid_cvc" => "The CVC number is incorrect.",
            "invalid_expiry_year" => "The expiration year invalid.",
            "invalid_number" => "The card number is incorrect.",
            "invalid_pin" => "The PIN entered is incorrect. This decline code only applies to payments made with a card reader.",
            "issuer_not_available" => "The card issuer could not be reached, so the payment could not be authorized.",
            "lost_card" => "The payment has been declined because the card is reported lost.",
            "merchant_blacklist" => "The payment has been declined because it matches a value on the Stripe user's block list.",
            "new_account_information_available" => "The card, or account the card is connected to, is invalid.",
            "no_action_taken" => "The card has been declined for an unknown reason.",
            "not_permitted" => "The payment is not permitted.",
            "pickup_card" => "The card cannot be used to make this payment (it is possible it has been reported lost or stolen).",
            "pin_try_exceeded" => "The allowable number of PIN tries has been exceeded.",
            "processing_error" => "An error occurred while processing the card.",
            "reenter_transaction" => "The payment could not be processed by the issuer for an unknown reason.",
            "restricted_card" => "The card cannot be used to make this payment (it is possible it has been reported lost or stolen).",
            "revocation_of_all_authorizations" => "The card has been declined for an unknown reason.",
            "revocation_of_authorization" => "The card has been declined for an unknown reason.",
            "security_violation" => "The card has been declined for an unknown reason.",
            "service_not_allowed" => "The card has been declined for an unknown reason.",
            "stolen_card" => "The payment has been declined because the card is reported stolen.",
            "stop_payment_order" => "The card has been declined for an unknown reason.",
            "testmode_decline" => "A Stripe test card number was used.",
            "transaction_not_allowed" => "The card has been declined for an unknown reason.",
            "try_again_later" => "The card has been declined for an unknown reason.",
            "withdrawal_count_limit_exceeded" => "The customer has exceeded the balance or credit limit available on their card.",
            _ => "Uknown error happened, please contact us.",
        };
    }
}