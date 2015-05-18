# StringStorage
I ran across a limitation in the in-game programming for Space Engineers, where I realized that variables are not persistent between executions. This led to a realization that there is indeed a persistent variable, namely a string named "Storage".

In my quest to enable more powerful scripting in-game, I made this parser program. It uses string operations for storing several variables in a single string, with methods for managing these variables.

Version 1.0 will be partially optimized, with a hashtable-buffered Read method able to perform 100k unique reads in under 0.02 seconds. The intention is to publish the code and subsequently work on optimizing the rest of the methods, like Update and Remove, to also be buffered.

The code will be available here in uncompressed form, however code intended for application in-game will be compressed, since there is a character limit for the in-game console. I'll set up separate branches for each, along with documentation in the wiki in case anyone would like to have some "proper" documentation. The compressed code will not have any comments. I intend to build documentation on the uncompressed code once it is completely optimized.
