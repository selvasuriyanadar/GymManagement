# Gym Management

  This is a payment management application.

  The features planned for this application include:
  - registering and viewing clients and their payments,
  - Ability to allow the clients to lent their purchase,
  - Notifying the application user about the payment statuses such as expiry, lent collection date, etc.,
  - keeping track of subscriptions,
  - cloud backup, bill printing.

## Technical Info

  Wherever possible 'Logic' has been extracted, such as HILogic, Logic, etc.
  Why 'Logic' and what exactly is ment by it here?
  'Logic' is a means to manipulate state in a controlled way thus reducing the possibility of bugs due to global state manipulations which is very easily possible in object oriented languages.
  Also 'Logic' is a means to store knowledge in a proper way into code so that knowledge is easily usable accross the application.
  What knowledge?
  The knowledge that we trade to our customers as applications. Applications are smart that is why they are being used.

  For example, the state of the payment can be modeled as a data structure and functions can be written that manipulate the data structure, to enable the payment state to transition from unpaid to paid, unpaid to lent and so on and so forth. With this we control the state to be manipulated in a controlled and predictable fashion. This removes complexity from the other pieces of code thus largely reducing bugs. But for this to happen we should program the functions that manipulate the state predictably which is the fun and may be the tough part of this approach.

  I have also programmed user interactions (HILogic) in a very similar fashion.

  I have always been a fan of Object orientation until I gained experience enough to appriciate functional approach. I no longer wish to touch the realms of object oriented programming. Even in C# I have kept my love for functional programming very much alive.

  To arrive at the perfect logic to code I sketch the problem and solve it in paper first then I will code.

  Wix has been used for creating installer for this application.

  Because I love vim and the command line, the application was developed completely from the command line.

### Some Coolest parts of the application that have been developed

  - Navigation
  - Payment Process
  - Classification of Payments by Alert Status
  - Customised Scroll Viewer (this may be unexpected but a lot of logic has been coded for this functionality)
  - Gui (I disliked MVVM so I am following a Gui crafting pattern very similar to that of react and flutter)
