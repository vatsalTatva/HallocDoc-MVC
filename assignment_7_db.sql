toc.dat                                                                                             0000600 0004000 0002000 00000007636 14612203351 0014450 0                                                                                                    ustar 00postgres                        postgres                        0000000 0000000                                                                                                                                                                        PGDMP   %    1                |            LibraryManagementSystem_db    16.1    16.1     �           0    0    ENCODING    ENCODING        SET client_encoding = 'UTF8';
                      false         �           0    0 
   STDSTRINGS 
   STDSTRINGS     (   SET standard_conforming_strings = 'on';
                      false         �           0    0 
   SEARCHPATH 
   SEARCHPATH     8   SELECT pg_catalog.set_config('search_path', '', false);
                      false         �           1262    49836    LibraryManagementSystem_db    DATABASE     �   CREATE DATABASE "LibraryManagementSystem_db" WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'English_India.1252';
 ,   DROP DATABASE "LibraryManagementSystem_db";
                postgres    false         �            1259    49844    book    TABLE     0  CREATE TABLE public.book (
    id integer NOT NULL,
    bookname character varying(128),
    author character varying(128),
    borrowerid integer,
    borrowername character varying(128),
    dateofissue timestamp without time zone,
    city character varying(128),
    genere character varying(128)
);
    DROP TABLE public.book;
       public         heap    postgres    false         �            1259    49843    book_id_seq    SEQUENCE     �   ALTER TABLE public.book ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.book_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);
            public          postgres    false    218         �            1259    49838    borrower    TABLE     [   CREATE TABLE public.borrower (
    id integer NOT NULL,
    city character varying(128)
);
    DROP TABLE public.borrower;
       public         heap    postgres    false         �            1259    49837    borrower_id_seq    SEQUENCE     �   ALTER TABLE public.borrower ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.borrower_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);
            public          postgres    false    216         �          0    49844    book 
   TABLE DATA           i   COPY public.book (id, bookname, author, borrowerid, borrowername, dateofissue, city, genere) FROM stdin;
    public          postgres    false    218       4844.dat �          0    49838    borrower 
   TABLE DATA           ,   COPY public.borrower (id, city) FROM stdin;
    public          postgres    false    216       4842.dat �           0    0    book_id_seq    SEQUENCE SET     :   SELECT pg_catalog.setval('public.book_id_seq', 10, true);
          public          postgres    false    217         �           0    0    borrower_id_seq    SEQUENCE SET     =   SELECT pg_catalog.setval('public.borrower_id_seq', 7, true);
          public          postgres    false    215         X           2606    49850    book pk_book 
   CONSTRAINT     J   ALTER TABLE ONLY public.book
    ADD CONSTRAINT pk_book PRIMARY KEY (id);
 6   ALTER TABLE ONLY public.book DROP CONSTRAINT pk_book;
       public            postgres    false    218         V           2606    49842    borrower pk_borrower 
   CONSTRAINT     R   ALTER TABLE ONLY public.borrower
    ADD CONSTRAINT pk_borrower PRIMARY KEY (id);
 >   ALTER TABLE ONLY public.borrower DROP CONSTRAINT pk_borrower;
       public            postgres    false    216         Y           2606    49851    book fk_book    FK CONSTRAINT     q   ALTER TABLE ONLY public.book
    ADD CONSTRAINT fk_book FOREIGN KEY (borrowerid) REFERENCES public.borrower(id);
 6   ALTER TABLE ONLY public.book DROP CONSTRAINT fk_book;
       public          postgres    false    218    216    4694                                                                                                          4844.dat                                                                                            0000600 0004000 0002000 00000000577 14612203351 0014263 0                                                                                                    ustar 00postgres                        postgres                        0000000 0000000                                                                                                                                                                        1	Whispher in the nights	Sarah BlackHood	1	Brinn	2024-04-24 00:00:00	Ahmedabad	Mystery
6	Book1	Author1	7	Borrower1	2024-04-24 00:00:00	Hyderabad	Fantasy
7	b3	a3	2	br3	2024-04-25 00:00:00	indore	Thriller
8	bb	aa	5	br	2024-04-25 00:00:00	Pune	Thriller
9	bb3	aa3	1	br3	2024-04-25 00:00:00	ahmedabad	History Fiction
10	book2	auth2	6	Borrower1	2024-04-25 00:00:00	Bangalore	Thriller
\.


                                                                                                                                 4842.dat                                                                                            0000600 0004000 0002000 00000000115 14612203351 0014245 0                                                                                                    ustar 00postgres                        postgres                        0000000 0000000                                                                                                                                                                        1	Ahmedabad
2	Indore
3	Jabalpur
4	Mumbai
5	Pune
6	Bangalore
7	Hyderabad
\.


                                                                                                                                                                                                                                                                                                                                                                                                                                                   restore.sql                                                                                         0000600 0004000 0002000 00000007545 14612203351 0015374 0                                                                                                    ustar 00postgres                        postgres                        0000000 0000000                                                                                                                                                                        --
-- NOTE:
--
-- File paths need to be edited. Search for $$PATH$$ and
-- replace it with the path to the directory containing
-- the extracted data files.
--
--
-- PostgreSQL database dump
--

-- Dumped from database version 16.1
-- Dumped by pg_dump version 16.1

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

DROP DATABASE "LibraryManagementSystem_db";
--
-- Name: LibraryManagementSystem_db; Type: DATABASE; Schema: -; Owner: postgres
--

CREATE DATABASE "LibraryManagementSystem_db" WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'English_India.1252';


ALTER DATABASE "LibraryManagementSystem_db" OWNER TO postgres;

\connect "LibraryManagementSystem_db"

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: book; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.book (
    id integer NOT NULL,
    bookname character varying(128),
    author character varying(128),
    borrowerid integer,
    borrowername character varying(128),
    dateofissue timestamp without time zone,
    city character varying(128),
    genere character varying(128)
);


ALTER TABLE public.book OWNER TO postgres;

--
-- Name: book_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.book ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.book_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: borrower; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.borrower (
    id integer NOT NULL,
    city character varying(128)
);


ALTER TABLE public.borrower OWNER TO postgres;

--
-- Name: borrower_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.borrower ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.borrower_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Data for Name: book; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.book (id, bookname, author, borrowerid, borrowername, dateofissue, city, genere) FROM stdin;
\.
COPY public.book (id, bookname, author, borrowerid, borrowername, dateofissue, city, genere) FROM '$$PATH$$/4844.dat';

--
-- Data for Name: borrower; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.borrower (id, city) FROM stdin;
\.
COPY public.borrower (id, city) FROM '$$PATH$$/4842.dat';

--
-- Name: book_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.book_id_seq', 10, true);


--
-- Name: borrower_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.borrower_id_seq', 7, true);


--
-- Name: book pk_book; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.book
    ADD CONSTRAINT pk_book PRIMARY KEY (id);


--
-- Name: borrower pk_borrower; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.borrower
    ADD CONSTRAINT pk_borrower PRIMARY KEY (id);


--
-- Name: book fk_book; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.book
    ADD CONSTRAINT fk_book FOREIGN KEY (borrowerid) REFERENCES public.borrower(id);


--
-- PostgreSQL database dump complete
--

                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           