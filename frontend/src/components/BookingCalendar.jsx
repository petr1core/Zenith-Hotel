// components/BookingCalendar.jsx
import React, { useState } from 'react';
import { Calendar, dateFnsLocalizer } from 'react-big-calendar';
import format from 'date-fns/format';
import parse from 'date-fns/parse';
import startOfWeek from 'date-fns/startOfWeek';
import getDay from 'date-fns/getDay';
import 'react-big-calendar/lib/css/react-big-calendar.css';
import { Paper, Typography } from '@mui/material';

const locales = {
  'en-US': require('date-fns/locale/en-US'),
};

const localizer = dateFnsLocalizer({
  format,
  parse,
  startOfWeek,
  getDay,
  locales,
});

const BookingCalendar = ({ bookings }) => {
  const [date, setDate] = useState(new Date());

  const events = bookings.map(booking => ({
    title: `Room ${booking.roomNumber} - ${booking.guestName}`,
    start: new Date(booking.checkInDate),
    end: new Date(booking.checkOutDate),
    allDay: true,
    resource: booking,
  }));

  const onSelectEvent = (event) => {
    // You can implement a modal to show booking details here
    console.log('Selected booking:', event.resource);
  };

  return (
    <Paper sx={{ p: 2, height: 600 }}>
      <Typography variant="h6" gutterBottom>Booking Calendar</Typography>
      <Calendar
        localizer={localizer}
        events={events}
        startAccessor="start"
        endAccessor="end"
        style={{ height: 500 }}
        onSelectEvent={onSelectEvent}
        date={date}
        onNavigate={setDate}
      />
    </Paper>
  );
};

export default BookingCalendar;